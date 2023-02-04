namespace backend.Data
{
    public class Tarina
    {
        //Public key
        public int TarinaId { get; set; }

        //Foreign key
        public int MatkakohdeId { get; set; }
        public virtual Matkakohde Matkakohde { get; set; }
        public int MatkaId { get; set; }
        public virtual Matka Matka { get; set; }

        //Items
        public DateTime Pvm { get; set; }
        public string Teksti { get; set; }
    }
}
