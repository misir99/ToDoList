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
        public string Opis { get; set; }
        public DateTime Rok { get; set; }
        public bool JeDovrsen { get; set; }
        public RazinaPrioriteta Prioritet { get; set; }  

        public Zadatak(string opis, DateTime rok, RazinaPrioriteta prioritet)
        {
            Opis = opis;
            Rok = rok;
            JeDovrsen = false; 
            Prioritet = prioritet;
        }
    }

    public class UpraviteljZadataka
    {
        private List<Zadatak> listaZadataka = new List<Zadatak>();

        public void DodajZadatak(Zadatak zadatak)
        {
            listaZadataka.Add(zadatak);
        }

        public void IspisiSveZadatke()
        {
            if (listaZadataka.Count == 0)
            {
                Console.WriteLine("nema zadataka.");
                return;
            }

            Console.WriteLine("\n--- TVOJA TO-DO LISTA ---");
           for(int i = 1; i <= listaZadataka.Count; i++)
            {
                Zadatak z = listaZadataka[i - 1];

            
                if(z.Rok < DateTime.Now && !z.JeDovrsen)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                }

                if (z.Prioritet == RazinaPrioriteta.Visok)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (z.Prioritet == RazinaPrioriteta.Srednji)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                string oznakaStatusa = z.JeDovrsen ? "[X]" : "[ ]";
                Console.WriteLine($"{i}. {oznakaStatusa} [{z.Prioritet}] {z.Opis} (Rok: {z.Rok.ToShortDateString()})");

                Console.ResetColor();


               
            }
            
            }

            public int DohvatiBrojZadataka()
            {
                return listaZadataka.Count;
            }


        public void OznaciKaoDovrsen(int broj)
        {
            if(broj< 1 ||broj > listaZadataka.Count)
            {
                Console.WriteLine("nesipravan broj zadatka.");
            }
            else
            {
                int indeks = broj - 1;

                listaZadataka[indeks].JeDovrsen = true;

                Console.WriteLine($"Zadtak `{listaZadataka[indeks].Opis}` je oznacen kao zavrsen! ✅");
            }
        }


        public void IzbrisiZadatak(int broj)
        {
            if(broj < 1 || broj > listaZadataka.Count)
            {
                Console.WriteLine("neispravan broj zadatka.");
            }

            listaZadataka.RemoveAt(broj - 1);
            Console.WriteLine("zadatak izbrisan!");
        }
        public bool JeLiListaPrazna()
        {
            return listaZadataka.Count == 0;
        }

        public void SpremiUDatoteku()
        {
            string putanja = "zadaci.txt";
            List<string> redci = new List<string>();

            foreach(var z in listaZadataka)
            {
                string redak = $"{z.Opis}|{z.Rok:dd/MM/yyyy}|{z.Prioritet}|{z.JeDovrsen}";
                redci.Add(redak);
            }
            File.WriteAllLines(putanja, redci);
        }

        public void UcitajIzDatoteke()
        {
            string putanja = "zadaci.txt";
            if (!File.Exists(putanja)) return;

            string[] redci = File.ReadAllLines(putanja);

            foreach(string redak in redci)
            {
                string[] dijelovi = redak.Split('|');

                if(dijelovi.Length == 4)
                {
                    string opis = dijelovi[0];
                    DateTime rok;
                    if (!DateTime.TryParse(dijelovi[1], out rok))
                    {
                        rok = DateTime.Now;
                    }
                    RazinaPrioriteta prioritet = (RazinaPrioriteta)Enum.Parse(typeof(RazinaPrioriteta), dijelovi[2]);
                    bool dovrsen = bool.Parse(dijelovi[3]);

                    Zadatak novi = new Zadatak(opis, rok, prioritet);
                    novi.JeDovrsen = dovrsen;

                    listaZadataka.Add(novi);
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





                        upravitelj.DodajZadatak(new Zadatak(opis, rok, odabraniPrioritet));
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
                            Console.WriteLine("\n--- TVOJI ZADACI ---");
                            upravitelj.IspisiSveZadatke();
                        }
                        break;
                        
                    case "3":
                        int broj;

                        if(upravitelj.JeLiListaPrazna())
                        {
                            Console.WriteLine("\nnema zadataka na listi.");
                            break;
                        }

                        Console.WriteLine("unesi broj zadatka koji zelis oznaciti zavrsenim!");
                        string brojInput = Console.ReadLine();

                        while (!int.TryParse(brojInput, out  broj) || broj < 1 || broj > upravitelj.DohvatiBrojZadataka())
                        {
                            Console.WriteLine($"Neispravan unos! Unesi broj od 1 do {upravitelj.DohvatiBrojZadataka()}:");
                            brojInput = Console.ReadLine();
                        }
                                                
                        upravitelj.OznaciKaoDovrsen(broj);
                        upravitelj.SpremiUDatoteku();

                        break;
                    case "4":
                        int brojZaBrisanje;
                        if(upravitelj.JeLiListaPrazna())
                        {
                            Console.WriteLine("\nnema zadataka na listi.");
                            break;
                        }

                        Console.WriteLine("unesi broj tadatka koji zelis izbrsisati");
                        string brojBrisanjeInput = Console.ReadLine();
                        while(!int.TryParse(brojBrisanjeInput, out brojZaBrisanje) || brojZaBrisanje < 1 || brojZaBrisanje > upravitelj.DohvatiBrojZadataka())
                        {
                            Console.WriteLine($"nesipravan unos! unesi broj od 1 do {upravitelj.DohvatiBrojZadataka()}:");
                            brojBrisanjeInput = Console.ReadLine();
                        }

                        upravitelj.IzbrisiZadatak(brojZaBrisanje);
                        upravitelj.SpremiUDatoteku();
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
