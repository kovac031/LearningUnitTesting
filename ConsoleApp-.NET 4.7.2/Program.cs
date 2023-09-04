using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

// instalirati nuget package PdfSharp za case 8

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args) // mora ici args jer mora biti nesto tu
        {
            string[] mainIzbornik = { "1. PARNOST", "2. KVADRATNA", "3. PROSJEK", "4. ZNAMENKE", "5. LOTO", "6. LISTIC", "7. OSOBA", "8. PDF", "\n" };
        
        izbornik: // goto
            Console.WriteLine("Odaberite jednu opciju:\n");
            int i = 0; // za ispis elemenata unutar arraya je potrebna petlja, ovo iznad su elementi u arrayu
            do
            {
                Console.WriteLine(mainIzbornik[i]);
                i++;
            }
            while (i < mainIzbornik.Length);

            string mainIzbor = Console.ReadLine(); //  ceka unos broja

            switch (mainIzbor)
            {
                case "1":
                    Console.WriteLine("Unesi broj po zelji da vidimo je li paran ili neparan:\n");
                    string parnost = Console.ReadLine();
                    int broj = Int32.Parse(parnost);
                    string result = (broj % 2 == 0) ? $"Broj {broj} je paran!\n" : $"Broj {broj} je neparan!\n";
                    Console.WriteLine(result);
                    goto izbornik;
                //--------------------------------------------------------------------
                case "2":
                opet:
                    Console.WriteLine("Unesi tri broja a, b, c kao parametre kvadratne jednadzbe:\n");

                    Console.WriteLine("\nUnesi a:");
                    string aStr = Console.ReadLine();
                    double a = Double.Parse(aStr);

                    Console.WriteLine("\nUnesi b:");
                    string bStr = Console.ReadLine();
                    double b = Double.Parse(bStr);

                    Console.WriteLine("\nUnesi c:");
                    string cStr = Console.ReadLine();
                    double c = Double.Parse(cStr);

                    var check = b * b - 4 * a * c;

                    if (check > 0)
                    {
                        double x1 = (-b + Math.Sqrt(check)) / 2 * a;
                        double x2 = (-b - Math.Sqrt(check)) / 2 * a;
                        Console.WriteLine($"\nRjesenja su {x1} i {x2}\n");
                    }
                    else
                    {
                        Console.WriteLine("S ovim brojevima će ispod korijena biti gadno. Daj neke druge brojeve.\n");
                        goto opet;
                    }

                    goto izbornik;
                //--------------------------------------------------------------------
                case "3":
                    Console.WriteLine("Unosite ocjene jednu po jednu za izračun prosjeka. Unosom nule se završi unos.\n");
                    List<int> unosOcjena = new List<int>();

                    do
                    {
                        string ocjenaStr = Console.ReadLine();
                        int ocjenaInt = Int32.Parse(ocjenaStr);
                        if (ocjenaInt != 0 && ocjenaInt <= 5 && ocjenaInt % 1 == 0)
                        {
                            unosOcjena.Add(ocjenaInt);
                        }
                        else if (ocjenaInt == 0)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("To nije ocjena. Probaj drugi broj:\n");
                        }
                    } while (true);
                    double prosjek = unosOcjena.Average();
                    string decimala = String.Format("{0:0.00}", prosjek);
                    Console.WriteLine($"Prosjek unesenih ocjena je {decimala}\n");
                    goto izbornik;
                //--------------------------------------------------------------------
                case "4":
                    Console.WriteLine("Prvo unesi n za n broj brojeva od kojih ću pozbrajti samo njihove zadnje znamenke:\n");
                    
                    string nStr = Console.ReadLine();
                    try
                    {
                        int nInt = Int32.Parse(nStr);
                        int zbroj = 0;
                        i = 0;
                        while (i < nInt)
                        {
                            Console.WriteLine($"\nUnesi {i + 1}. broj:");
                            string brojStr = Console.ReadLine();
                            string lastStr = brojStr.Substring(brojStr.Length - 1, 1);
                            int lastInt = Int32.Parse(lastStr);
                            zbroj = zbroj + lastInt;
                            i++;
                        }
                        Console.WriteLine($"Zbroj svih zadnjih znamenki je: {zbroj}!\n");
                    }
                    catch { Console.WriteLine("To nije prihvatljiv broj! (ako je decimalni, decimalu piši sa točkom)\n"); }
                    goto izbornik;
                //--------------------------------------------------------------------
                case "5":
                    Console.WriteLine("Odabrali ste LOTO! Sedam nasumičnih brojeva između 1 i 45 su:\n");

                    Loto();

                    foreach (int lotoBroj in lotoList)
                    {
                        Console.WriteLine(lotoBroj);
                    }

                    goto izbornik;
                //--------------------------------------------------------------------
                case "6":
                    try
                    {
                        string folderCase6 = "D:\\BEKEND\\Mono-Elpros-PracticeTasks\\Case6"; //napravi folder za case 6 ako ga nema

                        if (!Directory.Exists(folderCase6))
                        {
                            Directory.CreateDirectory(folderCase6);
                        }

                        string filePath = "D:\\BEKEND\\Mono-Elpros-PracticeTasks\\Case6\\LotoListic.txt";

                        if (!File.Exists(filePath))
                        {
                            FileStream stream = File.Create(filePath);
                            stream.Flush();
                            stream.Close();
                        }

                        File.WriteAllText(filePath, "Loto Listic\n");

                        int r = 0; // pocni od prvog retka
                        int br = 1; // prvi broj u nizu ce biti 1
                        while (r < 7)
                        {
                            int s = 0; // pocni od prvog stupca
                            while (s < 7) // predji u drugi red tek kad ispises 7 brojeva
                            {
                                File.AppendAllText(filePath, $"{br}\t");
                                s++;
                                br++;
                            }
                            File.AppendAllText(filePath, "\n"); // predji u novi red
                            r++; // zabiljezi da si sad u novom redu (trebat ce ih 7)
                        }
                        Console.WriteLine("\nLoto listic je kreiran.\n");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    goto izbornik;
                //--------------------------------------------------------------------
                case "7":
                    Console.WriteLine("\nUnesi 4 osobe, njihovo ime/prezime/datum rođenja/spol:");

                    List<Osoba> osobe = new List<Osoba>(); // add new class Osoba prvo

                    for (i = 1; i < 5; i++)
                    {
                        Console.WriteLine($"\nUnesi ime {i}. osobe:");
                        string ime = Console.ReadLine();
                        Console.WriteLine($"\nUnesi prezime {i}. osobe:");
                        string prezime = Console.ReadLine();
                        Console.WriteLine($"\nUnesi datum rođenja {i}. osobe u formatu dd.MM.yyyy. :");
                        string datumStr = Console.ReadLine();
                        Console.WriteLine($"\nUnesi spol {i}. osobe (M/F):");
                        string spol = Console.ReadLine();

                        if (DateTime.TryParseExact(datumStr, "dd.MM.yyyy.", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime datumDob))
                        {
                            Osoba osoba = new Osoba
                            {
                                FirstName = ime,
                                LastName = prezime,
                                DOB = datumDob,
                                Gender = spol
                            };
                            osobe.Add(osoba);
                        }
                        else
                        {
                            Console.WriteLine("Neispravan format datuma. Unesite datum u formatu dd.MM.yyyy.");
                        }
                    }
                    Osoba oldest = osobe.OrderBy(osoba => osoba.DOB).FirstOrDefault();
                    Console.WriteLine($"\nNajstarija osoba je {oldest.FirstName} {oldest.LastName}.");

                    Osoba youngest = osobe.OrderByDescending(osoba => osoba.DOB).FirstOrDefault();
                    Console.WriteLine($"\nNajmlađa osoba je {youngest.FirstName} {youngest.LastName}.");

                    int brM = osobe.Count(osoba => osoba.Gender == "M");
                    int brF = osobe.Count(osoba => osoba.Gender == "F");
                    Console.WriteLine($"\nUneseno je {brM} muške i {brF} ženske osobe");

                    int pre2000 = osobe.Count(osoba => osoba.DOB.Year < 2000);
                    Console.WriteLine($"\nUneseno je {pre2000} osobe rođenih prije 2000.");

                    goto izbornik;
                //--------------------------------------------------------------------
                case "8":
                    PdfDocument document = new PdfDocument();
                    PdfPage page = document.AddPage();

                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    XFont font = new XFont("Comic Sans", 30, XFontStyle.Bold);

                    Loto();

                    int x = 10;
                    int y = 10;

                    foreach (int lotoBroj in lotoList)
                    {
                        string lotoString = Convert.ToString(lotoBroj);

                        gfx.DrawString(
                        lotoString, font, XBrushes.DodgerBlue,
                        new XRect(x, y, page.Width, page.Height),
                        XStringFormats.TopCenter);
                        y += 40; // vidi dokumentaciju za PdfSharp za uredjivanje
                    }

                    string folderCase8 = "D:\\BEKEND\\Mono-Elpros-PracticeTasks\\Case8"; // provjera jel postoji, ne moze kreirati i folder i PDF automatski
                    if (!Directory.Exists(folderCase8))
                    {
                        Directory.CreateDirectory(folderCase8);
                    }
                    document.Save("D:\\BEKEND\\Mono-Elpros-PracticeTasks\\Case8\\LOTO.pdf");
                    Console.WriteLine("\nPDF je kreiran!\n");
                    goto izbornik;
                //--------------------------------------------------------------------
                default:
                    Console.WriteLine("Nije unesen jedan od ponuđenih brojeva:\n");
                    goto izbornik;
            }
            Console.ReadLine(); // drzi console otvoren
        }

        static List<int> lotoList = new List<int>(); // nova lista
        static void Loto() // metoda za case 5 i case 8, izbacivanje 7 nasumicnih brojeva od 1 do 45
        {
            Random random = new Random(); // Random je prebuilt klasa

            int i = 0;
            while (i < 7)
            {
                int broj = random.Next(1, 45);

                if (!lotoList.Contains(broj))
                {
                    lotoList.Add(broj);
                    i++;
                } // dok ih je manje od 7 (ali pocinje od 0), dodaj nasumicne na listu
            }
        }
    }
}
