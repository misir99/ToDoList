using System.Runtime.CompilerServices;

namespace ToDoList
{
    public enum RazinaPrioriteta
    {
        Nizak,
        Srednji,
        Visok
    }
   public class Zadatak
    {
        public int Id { get; set; }
        public string Opis { get; set; }
        public DateTime Rok { get; set; }
        public bool JeDovrsen { get; set; }
        public RazinaPrioriteta Prioritet { get; set; }  

        public Zadatak(int id, string opis, DateTime rok, RazinaPrioriteta prioritet)
        {
            Id = id;
            Opis = opis;
            Rok = rok;
            JeDovrsen = false; 
            Prioritet = prioritet;
        }
    }

    public class UpraviteljZadataka
    {
        private List<Zadatak> listaZadataka = new List<Zadatak>();
        private int sljedeciId = 1;

        public void DodajZadatak(string opis, DateTime rok, RazinaPrioriteta prioritet)
        {
            Zadatak novi = new Zadatak(sljedeciId++, opis, rok, prioritet);
            listaZadataka.Add(novi);
        }
        private ConsoleColor DohvatiBojuPrioriteta(RazinaPrioriteta prioritet)
        {
            switch (prioritet)
            {
                case RazinaPrioriteta.Visok:
                    return ConsoleColor.Red;
                case RazinaPrioriteta.Srednji:
                    return ConsoleColor.Yellow;
                case RazinaPrioriteta.Nizak:
                    return ConsoleColor.Gray;
                default:
                    return ConsoleColor.Gray;
            }
        }

        public void IspisiSveZadatke()
        {
            if (listaZadataka.Count == 0)
            {
                Console.WriteLine("nema zadataka.");
                return;
            }

            var sortiranaLista = listaZadataka.OrderBy(Zadatak => Zadatak.Rok).ToList();



            Console.WriteLine("\n--- TVOJA TO-DO LISTA ---");
           for(int i = 0; i < sortiranaLista.Count; i++)
            {
                Zadatak z = sortiranaLista[i];

                Console.ForegroundColor = DohvatiBojuPrioriteta(z.Prioritet);
                string porukaRoka = "";
                if (z.Rok.Date == DateTime.Today)
                {
                    porukaRoka = " -- DANAS! 🔥";
                }
                else if (z.Rok.Date < DateTime.Today)
                {
                    porukaRoka = " -- ROK PROŠAO! ⚠️";
                }
                string oznakaStatusa = z.JeDovrsen ? "[X]" : "[ ]";
                Console.WriteLine($"{z.Id}. {oznakaStatusa} [{z.Prioritet}] {z.Opis} (Rok: {z.Rok.ToShortDateString()}){porukaRoka}");

                Console.ResetColor();
            }            
            }
       
        public int DohvatiBrojZadataka()
            {
                return listaZadataka.Count;
            }


        public void OznaciKaoDovrsen(int idZaPretragu)
        {
            Zadatak z = listaZadataka.Find(x => x.Id == idZaPretragu);



            if(z != null)
            {
                z.JeDovrsen = true;
                Console.WriteLine($"Zadtak `{z.Opis}` je oznacen kao zavrsen! ✅");
            }
            else
            {
                Console.WriteLine("Žao mi je, ne postoji zadatak s tim brojem. ❌");
            }
        }


        public void IzbrisiZadatak(int idZaBrisanje)
        {
            Zadatak z = listaZadataka.Find(x => x.Id == idZaBrisanje);


            if (z != null)
            {
                listaZadataka.Remove(z);
                Console.WriteLine("Zadatak uspješno obrisan!");
              
            }
            else
            {
                Console.WriteLine("Zadatak s tim brojem ne postoji.");
            }
        }
        public bool JeLiListaPrazna()
        {
            return listaZadataka.Count == 0;
        }

        public void SpremiUDatoteku()
        {
            List<string> redci = new List<string>();
            foreach (Zadatak z in listaZadataka)
            {
               
                redci.Add($"{z.Id}|{z.Opis}|{z.Rok:dd/MM/yyyy}|{z.Prioritet}|{z.JeDovrsen}");
            }
            File.WriteAllLines("zadaci.txt", redci);
        }

        public void UcitajIzDatoteke()
        {
            string putanja = "zadaci.txt";
            if (!File.Exists(putanja)) return;

            string[] redci = File.ReadAllLines(putanja);
            int najveciId = 0;

            foreach(string redak in redci)
            {
                string[] dijelovi = redak.Split('|');

                if(dijelovi.Length == 5)
                {
                    int id = int.Parse(dijelovi[0]);
                    string opis = dijelovi[1];
                    DateTime rok = DateTime.Parse(dijelovi[2]);
                    RazinaPrioriteta prioritet = (RazinaPrioriteta)Enum.Parse(typeof(RazinaPrioriteta), dijelovi[3]);
                    bool dovrsen = bool.Parse(dijelovi[4]);

                    Zadatak novi = new Zadatak(id, opis, rok, prioritet);
                    novi.JeDovrsen = dovrsen;
                    listaZadataka.Add(novi);

                   
                    if (id > najveciId) najveciId = id;
                }

            }
            sljedeciId = najveciId + 1;
        }

        public void PretraziZadatke(string pojam)
        {
            var rezultati = listaZadataka.Where(z => z.Opis.ToLower().Contains(pojam.ToLower())).ToList();

            if(rezultati.Count == 0)
            {
                Console.WriteLine($"nema zadataka koji sadrze: `{pojam}`");
            }
            else
            {
                Console.WriteLine($"--- Rezultati pretrage za: `{pojam}` ---");
                foreach(var z in rezultati)
                {
                    string status = z.JeDovrsen ? "[X]" : "[ ]";
                    Console.WriteLine($"{z.Id}. {status} {z.Opis}");
                }
            }
        }
    }



    internal class Program
    {
        static void Main(string[] args)
        {
            UpraviteljZadataka upravitelj = new UpraviteljZadataka();

            upravitelj.UcitajIzDatoteke();

            bool ulaz = true;
            string izbor = "";
            while (izbor != "0" && izbor != "izlaz")
            {

                Console.WriteLine("\n1. dodaj zadatak.");
                Console.WriteLine("2. ispisi sve zadatke.");
                Console.WriteLine("3. oznaci zadatak kao dovrsen.");
                Console.WriteLine("4. izbrisi zadatak.");
                Console.WriteLine("5. pretrazi zadatke.");
                Console.WriteLine("0. izlaz.");
                Console.Write("odaberi opciju:");

                 izbor = Console.ReadLine().ToLower().Trim();
                
                switch (izbor)
                {
                    case "1":
                        Console.WriteLine("unesi opis zadatka:");
                        string opis = Console.ReadLine();

                        DateTime rok;
                        Console.WriteLine("unesi rok zadatka (dd/MM/yyyy):");
                        while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out rok))
                        {
                            Console.WriteLine("Neispravan format! Molim te unesi datum kao dd/MM/yyyy (npr. 20/04/2026):");
                        }

                        int p;
                        Console.WriteLine("unesi prioritet zadatka (0 - nizak, 1 - srednji, 2 - visok):");
                        string prioritetInput = Console.ReadLine();

                        while (!int.TryParse(prioritetInput, out p) || p < 0 || p > 2)
                        {
                            Console.WriteLine("Neispravan unos! Molim te unesi 0, 1 ili 2:");
                            prioritetInput = Console.ReadLine();

                        }

                        RazinaPrioriteta odabraniPrioritet = (RazinaPrioriteta)p;





                        upravitelj.DodajZadatak(opis, rok, odabraniPrioritet);
                        Console.WriteLine("zadatak dodan!");
                        upravitelj.SpremiUDatoteku();
                        break;
                    case "2":
                        if (upravitelj.JeLiListaPrazna())
                        {
                            Console.WriteLine("\nLista je trenutno prazna. Nema zadataka za prikaz! ✨");
                        }
                        else
                        {
                          
                            upravitelj.IspisiSveZadatke();
                        }
                        break;
                        
                    case "3":
                       
                        if(upravitelj.JeLiListaPrazna())
                        {
                            Console.WriteLine("\nNema zadataka za označavanje.");
                            break;
                        }

                        Console.WriteLine("Unesi ID zadatka koji želiš označiti kao dovršen:");
                       if(int.TryParse(Console.ReadLine(), out int idZaDovrsiti))
                        {
                            upravitelj.OznaciKaoDovrsen(idZaDovrsiti);
                            upravitelj.SpremiUDatoteku();
                        }
                        else
                        {
                            Console.WriteLine("Neispravan unos! Unesi točan ID!");
                        }
                            break;
                    case "4":
                        int brojZaBrisanje;
                        if(upravitelj.JeLiListaPrazna())
                        {
                            Console.WriteLine("\nnema zadataka na listi.");
                            break;
                        }

                        Console.WriteLine("unesi broj zadatka koji zelis izbrisati");
                        int idZaBrisanje;
                        while(!int.TryParse(Console.ReadLine(), out idZaBrisanje))
                        {
                            Console.WriteLine($"nesipravan unos! unesi broj od 1 do {upravitelj.DohvatiBrojZadataka()}:");
                           
                        }

                        upravitelj.IzbrisiZadatak(idZaBrisanje);
                        upravitelj.SpremiUDatoteku();
                        break;
                    case "5":
                        Console.WriteLine("unesi pojam za pretragu:");
                        string pojam = Console.ReadLine();
                        upravitelj.PretraziZadatke(pojam);
                        break;

                    case "0":
                    case "izlaz":
                        Console.WriteLine("izlaz iz programa. doviđenja!");
                        break;
                    default: 
                        Console.WriteLine("neispravan izbor, molim te pokusaj ponovo.");
                        break;


                }
            }

        }
    }
}
