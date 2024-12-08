namespace Trees.Models
{
    public class Magazyn
    {
        public int MagazynID { get; set; }
        public int GatunekID { get; set; }
        public int WielkoscID { get; set; }
        public int Ilosc { get; set; }
        public Gatunek Gatunek { get; set; }
        public Wielkosc Wielkosc { get; set; }
        public Stoisko Stoisko { get; set; }
    }
}
