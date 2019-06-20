using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading;
using System.Web.Mvc;
using GKHNNC.DAL;
using GKHNNC.Models;
using OpenQA.Selenium;//используем веб браузер для добычи файлов
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Remote;//для удаленки



namespace GKHNNC.Controllers
{
    
    public partial class AvtoScansController : Controller
    {
        
        public IWebDriver Browser;
        private WorkContext db = new WorkContext();


        public ActionResult Index()
        {

            return View();
        }
        
        public ActionResult GoToAvtoScan()
        {
            //Browser = new OpenQA.Selenium.Chrome.ChromeDriver();
            //Browser = new OpenQA.Selenium.IE.InternetExplorerDriver();
            //DesiredCapabilities capability = DesiredCapabilities.Firefox();
            

            DesiredCapabilities capability = DesiredCapabilities.Chrome();
            Uri url = new Uri("http://10.0.1.189:4545/wd/hub");
            IWebDriver Browser = new RemoteWebDriver(url, capability);
            //System.Environment.SetEnvironmentVariable("webdriver.chrome.driver", @"C:\\CHROMEWEBDRIVER\\chromedriver.exe");
            

            Browser.Manage().Window.Maximize();
            Browser.Navigate().GoToUrl("http://w.avtoscan.com/");

         //   IWebElement Login = Browser.FindElement(By.Id("gsr"));
            IWebElement Login = Browser.FindElement(By.Id("user"));
            IWebElement Password = Browser.FindElement(By.Id("passw"));
            IWebElement Submit = Browser.FindElement(By.Id("submit"));
            Login.SendKeys("$_ФГУП ЖКХ");
            Password.SendKeys("26072018");
            Submit.SendKeys( Keys.Enter);

            Browser.Manage().Timeouts().ImplicitlyWait( TimeSpan.FromSeconds(20));//ожидание загрузки 10 сек
            Thread.Sleep(1000);
            IWebElement CloseX = Browser.FindElement(By.Id("wizard_dlg_close"));
            Thread.Sleep(1000);
            CloseX.Click();
            
            IWebElement Otchets = Browser.FindElement(By.Id("hb_mi_reports_ctl"));
            Otchets.Click();
            //IWebElement Reports = Browser.FindElement(By.Id("report_templates_filter_reports"));
            //выбор элемента
            System.Threading.Thread.Sleep(1000);

            
                //IWebElement Reports = Browser.FindElement(By.Id("report_templates_filter_reports"));
                IWebElement Reports = Browser.FindElement(By.CssSelector("input[id='report_templates_filter_reports']"));
                //SelectElement S = new SelectElement(Browser.FindElement(By.CssSelector("input[id='report_templates_filter_reports']")));
                // Reports.SendKeys("КОМПЛЕКСНЫЙ ОТЧЕТ");
                Reports.Click();
                System.Threading.Thread.Sleep(1000);
                Reports = Browser.FindElement(By.CssSelector("div[value='17717313_1']"));
                Reports.Click();
                System.Threading.Thread.Sleep(1000);
            //чистим базу за этот день
            List<AutoScan> ASdb = db.AutoScans.Where(a => a.Date.Year == DateTime.Now.Year && a.Date.Month == DateTime.Now.Month && a.Date.Day == DateTime.Now.Day).ToList();
            foreach (AutoScan A in ASdb)
            {
                db.AutoScans.Remove(A);
                db.SaveChanges();
            }
            //грузим базу из инета
            string[] AllResult = new string[14];

            for (int j = 0; j < AllResult.Length; j++)
            {
               
                IWebElement Unit = Browser.FindElement(By.Id("report_templates_filter_units"));
                Unit.Click();
                System.Threading.Thread.Sleep(1000);

                Unit = Browser.FindElement(By.CssSelector("div[data-input-id='report_templates_filter_units']")).FindElement(By.CssSelector("li[idx='"+j.ToString()+"']"));
                
             
                Unit.Click();

                System.Threading.Thread.Sleep(1000);

                IWebElement Today = Browser.FindElement(By.XPath("//table//*[contains(text(), 'Сегодня')]"));
                Today.Click();

                Browser.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(20));
                string Result = "";
                for (int i = 0; i <= 20; i++)
                {
                    Unit = Browser.FindElement(By.CssSelector("tr[pos='" + i.ToString() + "']")).FindElement(By.XPath(".//td[2]"));
                    Result += Unit.Text + ";";
                }
                AllResult[j] = Result;
                string[] Res = Result.Split(';');

                AutoScan AS = new AutoScan();
                AS.Name = Res[0];
                if (Res[1].Contains("-")) { AS.Date = DateTime.Now; }
                else{AS.Date = Convert.ToDateTime(Res[1]);}
                string z = Res[3].Replace("км","");
                AS.KM = Convert.ToDecimal(z);
                AS.TimeInMove = Convert.ToDateTime(Res[4]);
                AS.MotoHours = Convert.ToDateTime(Res[5]);
                z = Res[6].Replace("км/ч", "");
                AS.MaxSpeed = Convert.ToDecimal(z);
                AS.Poesdki = Convert.ToDecimal(Res[7]);
                z = Res[12].Replace("л", "");
                AS.DUT = Convert.ToDecimal(z);
                z=Res[15].Replace("л", "");
                AS.Start = Convert.ToDecimal(z);
                z = Res[16].Replace("л", "");
                AS.End = Convert.ToDecimal(z);
                z = Res[17].Replace("л", "");
                AS.Zapravleno = Convert.ToDecimal(z);
               
                db.AutoScans.Add(AS);
                db.SaveChanges();
            }
            //SelectElement select = new SelectElement(Browser.FindElement(By.Id("report_templates_filter_reports"))); это не селект а инпут

            // SelectElement selector = new SelectElement(Reports);
            // selector.SelectByIndex(1);

            //

            string Data = "Данные успешно получены с сайта";
            Browser.Close();
            return Json(Data);
        }

    }
}