using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Trees.Models;

namespace Trees.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            ConnectionString connectionString = new ConnectionString(); // Tworzenie instancji klasy ConnectionString
            _connectionString = connectionString.DefaultConnection; // Uzyskanie dostępu do właściwości DefaultConnection
        }
        public async Task<Stoisko> GetStoiskoByIdAsync(int stoiskoId)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM Stoisko WHERE StoiskoID = @StoiskoID";
            return await connection.QueryFirstOrDefaultAsync<Stoisko>(sql, new { StoiskoID = stoiskoId });
        }

        public async Task<IEnumerable<Gatunek>> GetGatunkiAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Gatunek>("SELECT * FROM Gatunek");
        }

        public async Task<IEnumerable<Uzytkownicy>> GetUzytkownicyAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Uzytkownicy>("SELECT * FROM Uzytkownicy");
        }

        public async Task<IEnumerable<Wielkosc>> GetWielkosciAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Wielkosc>("SELECT * FROM Wielkosc");
        }

        public async Task<IEnumerable<Stoisko>> GetStoiskaAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Stoisko>("SELECT * FROM Stoisko");
        }



        public async Task<IEnumerable<Sprzedaz>> GetSprzedazeAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM Sprzedaz";
            return await connection.QueryAsync<Sprzedaz>(sql);
        }
        public async Task<float> GetTotalSprzedazByStoiskoAsync(int stoiskoId)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"SELECT SUM(Cena) 
                FROM Sprzedaz 
                WHERE StoiskoID = @StoiskoID"; // Filtruj sprzedaż dla wybranego stoiska
            var result = await connection.ExecuteScalarAsync<float>(sql, new { StoiskoID = stoiskoId }); // Przekazanie parametru
            return result;
        }
        public async Task<float> GetTotalSprzedazByStoiskoAndUserAsync(int stoiskoId, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"SELECT SUM(Cena) 
                FROM Sprzedaz 
                WHERE StoiskoID = @StoiskoID AND UserID =@UserID"; // Filtruj sprzedaż dla wybranego stoiska
            var result = await connection.ExecuteScalarAsync<float>(sql, new { StoiskoID = stoiskoId, UserID = userId }); // Przekazanie parametru
            return result;
        }





        public async Task<IEnumerable<Magazyn>> GetMagazynyAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Magazyn>("SELECT * FROM Magazyn");
        }
        public async Task<IEnumerable<WarehouseItem>> GetMagazynAsync(int stoiskoId)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"SELECT 
                    m.MagazynID,
                    m.ilosc, 
                    g.nazwagatunku, 
                    w.opiswielkosci 
                FROM Magazyn m
                INNER JOIN Gatunek g ON m.GatunekID = g.GatunekID
                INNER JOIN Wielkosc w ON m.WielkoscID = w.WielkoscID
                WHERE m.StoiskoID = @StoiskoID";

            return await connection.QueryAsync<WarehouseItem>(sql, new { StoiskoID = stoiskoId });
        }






        public async Task AddSprzedazAsync(Sprzedaz sprzedaz)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"INSERT INTO Sprzedaz (UserID, GatunekID, WielkoscID, Cena, DataSprzedazy, StoiskoID) 
                        VALUES (@UserID, @GatunekID, @WielkoscID, @Cena, @DataSprzedazy, @StoiskoID)";
            await connection.ExecuteAsync(sql, sprzedaz);
        }

        public async Task<Magazyn> GetMagazynAsync(int gatunekId, int wielkoscId, int stoiskoId)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"SELECT * FROM Magazyn WHERE GatunekID = @GatunekID AND WielkoscID = @WielkoscID AND StoiskoID = @StoiskoID";
            return await connection.QueryFirstOrDefaultAsync<Magazyn>(sql, new { GatunekID = gatunekId, WielkoscID = wielkoscId, StoiskoID = stoiskoId });
        }

        public async Task UpdateMagazynAsync(Magazyn magazyn)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"UPDATE Magazyn SET Ilosc = @Ilosc WHERE MagazynID = @MagazynID";
            await connection.ExecuteAsync(sql, new { Ilosc = magazyn.Ilosc, MagazynID = magazyn.MagazynID });
        }

        public async Task UpdateSprzedazAsync(Sprzedaz nowaSprzedaz)
        {
            using var connection = new SqlConnection(_connectionString);


            var staraSprzedaz = await connection.QueryFirstOrDefaultAsync<Sprzedaz>(
                "SELECT * FROM Sprzedaz WHERE SprzedazID = @SprzedazID",
                new { SprzedazID = nowaSprzedaz.SprzedazID });


            var magazyn = await GetMagazynAsync(nowaSprzedaz.GatunekID, nowaSprzedaz.WielkoscID, nowaSprzedaz.StoiskoID);

            if (magazyn != null)
            {




                if (magazyn.Ilosc + 1 < 0)
                {
                    throw new InvalidOperationException("Niewystarczająca ilość w magazynie, aby zrealizować tę zmianę.");
                }

                var query = "UPDATE Sprzedaz SET Cena = @Cena WHERE SprzedazID = @SprzedazID";
                await connection.ExecuteAsync(query, new
                {
                    Cena = nowaSprzedaz.Cena,

                    SprzedazID = nowaSprzedaz.SprzedazID
                });


                magazyn.Ilosc += 1;

                await UpdateMagazynAsync(magazyn);
            }
        }


        public async Task DeleteSprzedazAsync(int sprzedazId)
        {
            using var connection = new SqlConnection(_connectionString);


            var sprzedaz = await connection.QueryFirstOrDefaultAsync<Sprzedaz>(
                "SELECT * FROM Sprzedaz WHERE SprzedazID = @SprzedazID", new { SprzedazID = sprzedazId });

            if (sprzedaz != null)
            {

                var magazyn = await GetMagazynAsync(sprzedaz.GatunekID, sprzedaz.WielkoscID, sprzedaz.StoiskoID);

                if (magazyn != null)
                {

                    magazyn.Ilosc += 1;
                    await UpdateMagazynAsync(magazyn);
                }


                var sql = @"DELETE FROM Sprzedaz WHERE SprzedazID = @SprzedazID";
                await connection.ExecuteAsync(sql, new { SprzedazID = sprzedazId });
            }
        }


        public async Task<IEnumerable<Sprzedaz>> GetSprzedazWithDetailsAndUserAsync(Stoisko stoisko, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
        SELECT s.Cena,s.DataSprzedazy, g.NazwaGatunku, w.OpisWielkosci, u.Login, st.StoiskoNazwa,s.sprzedazID,s.gatunekID,s.stoiskoID,s.WielkoscID 
        FROM Sprzedaz s
        INNER JOIN Gatunek g ON s.GatunekID = g.GatunekID
        INNER JOIN Wielkosc w ON s.WielkoscID = w.WielkoscID
        INNER JOIN Uzytkownicy u ON s.UserID = u.UserID
        INNER JOIN Stoisko st ON s.StoiskoID = st.StoiskoID
        WHERE s.StoiskoID = @StoiskoID AND s.UserID=@UserID";




            var parameters = new { StoiskoID = stoisko.StoiskoID, UserID = userId };

            return await connection.QueryAsync<Sprzedaz>(sql, parameters);
        }
        public async Task<float> GetTotalSprzedazByStoiskoAndDateAsync(int stoiskoId, DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"SELECT SUM(Cena) 
                FROM Sprzedaz 
                WHERE StoiskoID = @StoiskoID AND CAST(DataSprzedazy AS DATE) = @Date";
            return await connection.ExecuteScalarAsync<float>(sql, new { StoiskoID = stoiskoId, Date = date.Date });
        }
        public async Task<float> GetTotalSprzedazByStoiskoAndUserAndDateAsync(int stoiskoId, int userId, DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"SELECT SUM(Cena) 
                FROM Sprzedaz 
                WHERE StoiskoID = @StoiskoID AND UserID = @UserID AND CAST(DataSprzedazy AS DATE) = @Date";
            return await connection.ExecuteScalarAsync<float>(sql, new { StoiskoID = stoiskoId, UserID = userId, Date = date.Date });
        }

        public async Task<IEnumerable<Sprzedaz>> GetSprzedazWithDetailsAndUserAndDateAsync(Stoisko stoisko, int userId, DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
        SELECT s.Cena, s.DataSprzedazy, g.NazwaGatunku, w.OpisWielkosci, u.Login, st.StoiskoNazwa, s.SprzedazID, s.GatunekID, s.StoiskoID, s.WielkoscID
        FROM Sprzedaz s
        INNER JOIN Gatunek g ON s.GatunekID = g.GatunekID
        INNER JOIN Wielkosc w ON s.WielkoscID = w.WielkoscID
        INNER JOIN Uzytkownicy u ON s.UserID = u.UserID
        INNER JOIN Stoisko st ON s.StoiskoID = st.StoiskoID
        WHERE s.StoiskoID = @StoiskoID AND s.UserID = @UserID AND CAST(s.DataSprzedazy AS DATE) = @Date order by s.sprzedazID desc";
            return await connection.QueryAsync<Sprzedaz>(sql, new { StoiskoID = stoisko.StoiskoID, UserID = userId, Date = date.Date });
        }
        public async Task<IEnumerable<Sprzedaz>> GetSprzedazWithDetailsAndDateAsync(Stoisko stoisko, DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
        SELECT s.Cena, s.DataSprzedazy, g.NazwaGatunku, w.OpisWielkosci, u.Login, st.StoiskoNazwa, s.SprzedazID, s.GatunekID, s.StoiskoID, s.WielkoscID
        FROM Sprzedaz s
        INNER JOIN Gatunek g ON s.GatunekID = g.GatunekID
        INNER JOIN Wielkosc w ON s.WielkoscID = w.WielkoscID
        INNER JOIN Stoisko st ON s.StoiskoID = st.StoiskoID
        INNER JOIN Uzytkownicy u ON s.UserID = u.UserID

        WHERE s.StoiskoID = @StoiskoID AND CAST(s.DataSprzedazy AS DATE) = @Date order by s.sprzedazID desc ";
            return await connection.QueryAsync<Sprzedaz>(sql, new { StoiskoID = stoisko.StoiskoID, Date = date.Date });
        }
        public async Task<IEnumerable<Sprzedaz>> GetSprzedazWithDetailsAsync(Stoisko stoisko)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
        SELECT s.Cena,s.DataSprzedazy, g.NazwaGatunku, w.OpisWielkosci, u.Login, st.StoiskoNazwa,s.sprzedazID,s.gatunekID,s.stoiskoID,s.WielkoscID 
        FROM Sprzedaz s
        INNER JOIN Gatunek g ON s.GatunekID = g.GatunekID
        INNER JOIN Wielkosc w ON s.WielkoscID = w.WielkoscID
        INNER JOIN Uzytkownicy u ON s.UserID = u.UserID
        INNER JOIN Stoisko st ON s.StoiskoID = st.StoiskoID
        WHERE s.StoiskoID = @StoiskoID order by s.sprzedazID desc";




            var parameters = new { StoiskoID = stoisko.StoiskoID };

            return await connection.QueryAsync<Sprzedaz>(sql, parameters);
        }





    }
}
