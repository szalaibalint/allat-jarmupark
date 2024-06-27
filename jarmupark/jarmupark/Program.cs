using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace jarmupark
{
    abstract class Jarmu
    {
        public string Azonosito { get; private set; }
        public string Rendszam { get; set; }
        public int GyartasiEv { get; private set; }
        public double Fogyasztas { get; set; }
        public double FutottKm { get; private set; }
        public int AktualisKoltseg { get; private set; }
        public bool Szabad { get; private set; }
        public static int AktualisEv { get; set; }
        public static int AlapDij { get; set; }
        public static double HaszonKulcs { get; set; }

        public Jarmu(string azonosito, string rendszam, int gyartasiEv, double fogyasztas)
        {
            this.Azonosito = azonosito;
            this.Rendszam = rendszam;
            this.GyartasiEv = gyartasiEv;
            this.Fogyasztas = fogyasztas;
            this.Szabad = true;
        }
        public Jarmu(string azonosito, string rendszam, int gyartasiEv)
        {
            this.Azonosito = azonosito;
            this.Rendszam = rendszam;
            this.GyartasiEv = gyartasiEv;
            this.Szabad = true;
        }

        public int Kor()
        {
            return AktualisEv - GyartasiEv;
        }

        public bool Fuvaroz(double ut, int benzinAr)
        {
            if (Szabad)
            {
                FutottKm += ut;
                AktualisKoltseg = (int)(benzinAr * ut * Fogyasztas / 100);
                Szabad = false;
                return true;
            }
            return false;
        }

        public virtual int BerletiDij()
        {
            return (int)(AlapDij + AktualisKoltseg + AktualisKoltseg * HaszonKulcs / 100);
        }

        public void Vegzett()
        {
            Szabad = true;
        }

        public override string ToString()
        {
            return "\nA " + this.GetType().Name.ToLower() +
                " azonosítója: " + Azonosito +
                "\nrendszáma: " + Rendszam +
                "\nkora: " + Kor() + " év" +
                "\nfogyasztása: " + Fogyasztas + " l/100 km" +
                "\na km-óra állása: " + FutottKm + " km";
        }
    }

    class Busz : Jarmu
    {
        public int Ferohely { get; private set; }
        public static double Szorzo { get; set; }

        public Busz(string azonosito, string rendszam, int gyartasiEv, double fogyasztas, int ferohely) :
            base(azonosito, rendszam, gyartasiEv, fogyasztas)
        {
            this.Ferohely = ferohely;
        }
        public Busz(string azonosito, string rendszam, int gyartasiEv, int ferohely) :
            base(azonosito, rendszam, gyartasiEv)
        {
            this.Ferohely = ferohely;
        }

        public override int BerletiDij()
        {
            return (int)(base.BerletiDij() + Ferohely * Szorzo);
        }

        public override string ToString()
        {
            return base.ToString() +
                "\nférőhelyek száma: " + Ferohely;
        }
    }

    class TeherAuto : Jarmu
    {
        public double TeherBiras { get; private set; }
        public static double Szorzo { get; set; }

        public TeherAuto(string azonosito, string rendszam, int gyartasiEv, double fogyasztas, double teherBiras) :
            base(azonosito, rendszam, gyartasiEv, fogyasztas)
        {
            this.TeherBiras = teherBiras;
        }
        public TeherAuto(string azonosito, string rendszam, int gyartasiEv, double teherBiras) :
            base(azonosito, rendszam, gyartasiEv)
        {
            this.TeherBiras = teherBiras;
        }

        public override int BerletiDij()
        {
            return (int)(base.BerletiDij() + TeherBiras * Szorzo);
        }

        public override string ToString()
        {
            return base.ToString() +
                "\nteherbírás: " + TeherBiras + " tonna";
        }
    }

    class Vezerles
    {
        private List<Jarmu> jarmuvek = new List<Jarmu>();
        private string BUSZ = "busz";
        private string TEHER_AUTO = "teherautó";

        public void Indit()
        {
            StatikusBeallitas();
            AdatBevitel();
            Kiir("A regisztrált járművek: ");
            Mukodtet();
            Kiir("\nA működés utáni állapot: ");
            AtlagKor();
            LegtobbKilometer();
            Rendez();
        }

        private void StatikusBeallitas()
        {
            Jarmu.AktualisEv = 2015;
            Jarmu.AlapDij = 1000;
            Jarmu.HaszonKulcs = 10;
            Busz.Szorzo = 15;
            TeherAuto.Szorzo = 8.5;
        }

        private void AdatBevitel()
        {
            string tipus, rendszam, azonosito;
            int gyartEv, ferohely;
            double fogyasztas, teherbiras;

            StreamReader olvasoCsatorna = new StreamReader("jarmuvek.txt");

            int sorszam = 1;

            while (!olvasoCsatorna.EndOfStream)
            {
                tipus = olvasoCsatorna.ReadLine();
                rendszam = olvasoCsatorna.ReadLine();
                gyartEv = Int32.Parse(olvasoCsatorna.ReadLine());
                fogyasztas = double.Parse(olvasoCsatorna.ReadLine());
                azonosito = tipus.Substring(0, 1).ToUpper() + sorszam;

                if (tipus == BUSZ)
                {
                    ferohely = Int32.Parse(olvasoCsatorna.ReadLine());
                    jarmuvek.Add(new Busz(azonosito, rendszam, gyartEv, fogyasztas, ferohely));
                }
                else if (tipus == TEHER_AUTO)
                {
                    teherbiras = double.Parse(olvasoCsatorna.ReadLine());
                    jarmuvek.Add(new TeherAuto(azonosito, rendszam, gyartEv, fogyasztas, teherbiras));
                }
                sorszam++;
            }
            olvasoCsatorna.Close();
        }

        private void Kiir(string cim)
        {
            Console.WriteLine(cim);
            foreach (Jarmu jarmu in jarmuvek)
            {
                Console.WriteLine(jarmu);
            }
        }

        private void Mukodtet()
        {
            int osszKoltseg = 0, osszBevetel = 0;

            Random rand = new Random();
            int alsoBenzinAr = 400, felsoBenzinAr = 420;
            double utMax = 300;
            int mukodesHatar = 200;
            int jarmuIndex;

            Jarmu jarmu;
            int fuvarSzam = 0;

            for (int i = 0; i < (int)rand.Next(mukodesHatar); i++)
            {
                jarmuIndex = rand.Next(jarmuvek.Count);
                jarmu = jarmuvek[jarmuIndex];
                if (jarmu.Fuvaroz(rand.NextDouble() * utMax, rand.Next(alsoBenzinAr, felsoBenzinAr)))
                {
                    fuvarSzam++;
                    Console.WriteLine("\nAz induló jármű rendszáma: " + jarmu.Rendszam
                        + "\nAz aktuális fuvarozási költség: " + jarmu.AktualisKoltseg + " Ft."
                        + "\nAz aktuális bevétel: " + jarmu.BerletiDij() + " Ft.");
                    osszBevetel += jarmu.BerletiDij();
                    osszKoltseg += jarmu.AktualisKoltseg;
                }
                jarmuIndex = rand.Next(jarmuvek.Count);
                jarmuvek[jarmuIndex].Vegzett();
            }
            Console.WriteLine("\n\nA cég teljes költsége: " + osszKoltseg + " Ft." +
                "\n\nTeljes bevétele: " + osszBevetel + " Ft." +
                "\n\nHaszna: " + (osszBevetel - osszKoltseg) + " Ft.");
            Console.WriteLine("\nA fuvarok száma: " + fuvarSzam);
        }

        private void AtlagKor()
        {
            double osszKor = 0;
            int darab = 0;
            foreach (Jarmu jarmu in jarmuvek)
            {
                osszKor += jarmu.Kor();
                darab++;
            }

            if (darab > 0)
            {
                Console.WriteLine("\nA járművek átlag kora: " + osszKor / darab + " év.");
            }
            else
            {
                Console.WriteLine("Nincsenek járművek.");
            }
        }

        private void LegtobbKilometer()
        {
            double max = jarmuvek[0].FutottKm;
            foreach (Jarmu jarmu in jarmuvek)
            {
                if (jarmu.FutottKm > max)
                {
                    max = jarmu.FutottKm;
                }
            }
            Console.WriteLine("\nA legtöbbet futott jármű(vek) {0: 000.00} km-rel:", max);
            foreach (Jarmu jarmu in jarmuvek)
            {
                if (jarmu.FutottKm == max)
                {
                    Console.WriteLine(jarmu.Rendszam);
                }
            }
        }

        private void Rendez()
        {
            Jarmu temp;

            for (int i = 0; (i < jarmuvek.Count - 1); i++)
            {
                for (int j = i + 1; j < jarmuvek.Count; j++)
                {
                    if (jarmuvek[i].Fogyasztas > jarmuvek[j].Fogyasztas)
                    {
                        temp = jarmuvek[i];
                        jarmuvek[i] = jarmuvek[j];
                        jarmuvek[j] = temp;
                    }
                }
            }
            Console.WriteLine("\nA járművek fogyasztás szerint rendezve: ");

            foreach (Jarmu jarmu in jarmuvek)
            {
                Console.WriteLine("{0,-10} {1:00.0} liter / 100 km.", jarmu.Rendszam, jarmu.Fogyasztas);
            }
        }
    }

        internal class Program
    {
        static void Main(string[] args)
        {
            new Vezerles().Indit();

            Console.ReadKey();
        }
    }
}
