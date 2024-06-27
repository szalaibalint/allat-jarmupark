using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace allat
{
    class Allat
    {
        public string Nev { get; private set; }
        public int SzuletesiEv { get; private set; }
        public int RajtSzam { get; private set; }
        public int SzepsegPont { get; private set; }
        public int ViselkedesPont { get; private set; }
        public int PontSzam { get; protected set; }
        public static int AktualisEv { get; set; }
        public static int KorHatar { get; set; }

        public Allat(int rajtSzam, string nev, int szuletesiEv)
        {
            RajtSzam = rajtSzam;
            Nev = nev;
            SzuletesiEv = szuletesiEv;
        }

        public int Kor()
        {
            return AktualisEv - SzuletesiEv;
        }

        public virtual void Pontozzak(int szepsegPont, int viselkedesPont)
        {
            this.SzepsegPont = szepsegPont;
            this.ViselkedesPont = viselkedesPont;
            if (Kor() < KorHatar)
            {
                PontSzam = viselkedesPont * Kor() + szepsegPont * (KorHatar - Kor());
            }
            else
            {
                PontSzam = 0;
            }
        }

        public override string ToString()
        {
            return RajtSzam + ". " + Nev + " nevű "
                + this.GetType().Name.ToLower()
                + " pontszáma: " + PontSzam + " pont";
        }
    }

    class Kutya : Allat
    {
        public int GazdaViszonyPont { get; private set; }
        public bool KapottViszonyPontot { get; private set; }

        public Kutya(int rajtSzam, string nev, int szuletesiEv) :
            base(rajtSzam, nev, szuletesiEv) { }

        public void ViszonyPontozas(int gazdaViszonyPont)
        {
            this.GazdaViszonyPont = gazdaViszonyPont;
            KapottViszonyPontot = true;
        }

        public override void Pontozzak(int szepsegPont, int viselkedesPont)
        {
            if (KapottViszonyPontot)
            {
                base.Pontozzak(szepsegPont , viselkedesPont);
                this.PontSzam += GazdaViszonyPont;
            }
        }
    }

    class Macska : Allat
    {
        public bool VanMacskaSzallitoDoboz { get; set; }

        public Macska(int rajtSzam, string nev, int szuletesiEv, bool vanMacskaSzallitoDoboz) :
            base(rajtSzam, nev, szuletesiEv)
        {
            this.VanMacskaSzallitoDoboz = vanMacskaSzallitoDoboz;
        }

        public override void Pontozzak(int szepsegPont, int viselkedesPont)
        {
            if (VanMacskaSzallitoDoboz)
            {
                base.Pontozzak(szepsegPont, viselkedesPont);
            }
        }
    }

    class Vezerles
    {
        private List<Allat> allatok = new List<Allat>();

        public void Start()
        {
            Allat.AktualisEv = 2015;
            Allat.KorHatar = 10;

            //Proba();

            Regisztracio();
            Kiiratas("A regisztrált versenyzők");
            Verseny();
            Kiiratas("\nA verseny eredménye");
        }

        private void Proba()
        {
            Allat allat1, allat2;

            string nev1 = "Pamacs", nev2 = "Bolhazsák";
            int szulEv1 = 2010, szulEv2 = 2011;
            bool vanDoboz = true;
            int rajtSzam = 1;

            int szepsegPont = 5,
                viselkedesPont = 4,
                viszonyPont = 6;

            allat1 = new Kutya(rajtSzam, nev1, szulEv1);
            rajtSzam++;
            allat2 = new Macska(rajtSzam, nev2, szulEv2, vanDoboz);

            Console.WriteLine("A regisztrált állatok:");
            Console.WriteLine(allat1);
            Console.WriteLine(allat2);

            if (allat1 is Kutya)
            {
                (allat1 as Kutya).ViszonyPontozas(viszonyPont);
            }
            allat1.Pontozzak(szepsegPont, viselkedesPont);
            allat2.Pontozzak(szepsegPont, viselkedesPont);

            Console.WriteLine("\nA verseny eredménye:");
            Console.WriteLine(allat1);
            Console.WriteLine(allat2);
        }

        private void Regisztracio()
        {
            StreamReader olvasoCsatorna = new StreamReader("allatok.txt");

            string fajta, nev;
            int rajtSzam = 1, szulEv;
            bool vanDoboz;

            while (!olvasoCsatorna.EndOfStream)
            {
                fajta = olvasoCsatorna.ReadLine();
                nev = olvasoCsatorna.ReadLine();
                szulEv = Int32.Parse(olvasoCsatorna.ReadLine());

                if (fajta == "kutya")
                {
                    allatok.Add(new Kutya(rajtSzam, nev, szulEv));
                }
                else
                {
                    vanDoboz = bool.Parse(olvasoCsatorna.ReadLine());
                    allatok.Add(new Macska(rajtSzam, nev, szulEv, vanDoboz));
                }
                rajtSzam++;
            }
            olvasoCsatorna.Close();
        }

        private void Verseny()
        {
            Random rand = new Random();
            int hatar = 11;
            foreach (Allat item in allatok)
            {
                if (item is Kutya)
                {
                    (item as Kutya).ViszonyPontozas(rand.Next(hatar));
                }
                item.Pontozzak(rand.Next(hatar), rand.Next(hatar));
            }
        }

        private void Kiiratas(string cim)
        {
            Console.WriteLine(cim);
            foreach (Allat item in allatok)
            {
                Console.WriteLine(item);
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            new Vezerles().Start();

            Console.ReadKey();
        }
    }
}
