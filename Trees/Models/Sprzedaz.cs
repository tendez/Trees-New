namespace Trees.Models
{
    public class Sprzedaz
    {
        public int SprzedazID { get; set; }
        public int UserID { get; set; }
        public int GatunekID { get; set; }
        public int WielkoscID { get; set; }
        public decimal Cena { get; set; }
        public DateTime DataSprzedazy { get; set; }
        public int StoiskoID { get; set; }


        public string NazwaGatunku { get; set; }
        public string OpisWielkosci { get; set; }
        public string Login { get; set; }
        public string StoiskoNazwa { get; set; }
    }
}
