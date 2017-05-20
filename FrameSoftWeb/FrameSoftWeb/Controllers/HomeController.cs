using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FrameSoftWeb.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        static string State = "";
        static string ErrorMessage = "";
        public ActionResult RestoreDataBaseNow()
        {
            try
            {

                //if (!string.IsNullOrEmpty(ErrorMessage))
                //    return Content(ErrorMessage);
                //if (!string.IsNullOrEmpty(State))
                //    return Content(State);
                //Task task = new Task(() =>
                //{
                //    try
                //    {

                //        //State = "Connection to LocalHost Amars...";
                //        //List<AmarTable> amars = new List<AmarTable>();
                //        //using (var ctx = new AmarContext("Data Source=framesoft.ir;Integrated Security=False;Initial Catalog=FrameSoftAmar;User ID=ali;Password=hamishebahar?3773284;Connect Timeout=15;Encrypt=False;Packet Size=4096"))
                //        //{
                //        //    State = "Selecting Amars from LocalHost...";
                //        //    amars = (from x in ctx.Amars select x).ToList();
                //        //}
                //        //State = "Connection to VPS Amars... " + amars.Count;
                //        //using (var ctx = new AmarContext("Data Source=178.32.84.95;Initial Catalog=FrameSoftAmar2;User ID=Ali;Password=hamis242he%BAHTCPASS?5848"))
                //        //{
                //        //    State = "Clearing Amars from VPS...";
                //        //    ctx.Amars.RemoveRange(ctx.Amars);
                //        //    State = "Inserting Amars from VPS..." + amars.Count;
                //        //    ctx.Amars.AddRange(amars);
                //        //    State = "Saving Amars from VPS..." + amars.Count;
                //        //    ctx.SaveChanges();
                //        //}

                //        //State = "Connection to LocalHost appnames...";
                //        //List<ApplicationNamesTable> appnames = new List<ApplicationNamesTable>();
                //        //using (var ctx = new AmarContext("Data Source=framesoft.ir;Integrated Security=False;Initial Catalog=FrameSoftAmar;User ID=ali;Password=hamishebahar?3773284;Connect Timeout=15;Encrypt=False;Packet Size=4096"))
                //        //{
                //        //    State = "Selecting appnames from LocalHost...";
                //        //    appnames = (from x in ctx.ApplicationNames select x).ToList();
                //        //}

                //        //State = "Connection to VPS appnames... " + appnames.Count;
                //        //using (var ctx = new AmarContext("Data Source=178.32.84.95;Initial Catalog=FrameSoftAmar2;User ID=Ali;Password=hamis242he%BAHTCPASS?5848"))
                //        //{
                //        //    State = "Clearing appnames from VPS...";
                //        //    ctx.ApplicationNames.RemoveRange(ctx.ApplicationNames);
                //        //    State = "Inserting appnames from VPS..." + appnames.Count;
                //        //    ctx.ApplicationNames.AddRange(appnames);
                //        //    State = "Saving Amars from VPS..." + appnames.Count;
                //        //    ctx.SaveChanges();
                //        //}


                //        //State = "Connection to LocalHost ApplicationVersions...";
                //        //List<ApplicationVersionTable> ApplicationVersions = new List<ApplicationVersionTable>();
                //        //using (var ctx = new AmarContext("Data Source=framesoft.ir;Integrated Security=False;Initial Catalog=FrameSoftAmar;User ID=ali;Password=hamishebahar?3773284;Connect Timeout=15;Encrypt=False;Packet Size=4096"))
                //        //{
                //        //    State = "Selecting ApplicationVersions from LocalHost...";
                //        //    ApplicationVersions = (from x in ctx.ApplicationVersions select x).ToList();
                //        //}

                //        //State = "Connection to VPS ApplicationVersions... " + ApplicationVersions.Count;
                //        //using (var ctx = new AmarContext("Data Source=178.32.84.95;Initial Catalog=FrameSoftAmar2;User ID=Ali;Password=hamis242he%BAHTCPASS?5848"))
                //        //{
                //        //    State = "Clearing ApplicationVersions from VPS...";
                //        //    ctx.ApplicationVersions.RemoveRange(ctx.ApplicationVersions);
                //        //    State = "Inserting ApplicationVersions from VPS..." + ApplicationVersions.Count;
                //        //    ctx.ApplicationVersions.AddRange(ApplicationVersions);
                //        //    State = "Saving ApplicationVersions from VPS..." + ApplicationVersions.Count;
                //        //    ctx.SaveChanges();
                //        //}

                //        State = "Connection to LocalHost GuidDetails...";
                //        List<GuidDetailsTable> GuidDetails = new List<GuidDetailsTable>();
                //        using (var ctx = new AmarContext("Data Source=framesoft.ir;Integrated Security=False;Initial Catalog=FrameSoftAmar;User ID=ali;Password=hamishebahar?3773284;Connect Timeout=15;Encrypt=False;Packet Size=4096"))
                //        {
                //            State = "Selecting GuidDetails from LocalHost...";
                //            GuidDetails = (from x in ctx.GuidDetails select x).ToList();
                //        }

                //        State = "Connection to VPS GuidDetails... " + GuidDetails.Count;
                //        using (var ctx = new AmarContext("Data Source=178.32.84.95;Initial Catalog=FrameSoftAmar2;User ID=Ali;Password=hamis242he%BAHTCPASS?5848"))
                //        {
                //            //State = "Clearing GuidDetails from VPS...";
                //            //ctx.GuidDetails.RemoveRange(ctx.GuidDetails);
                //            State = "Inserting GuidDetails from VPS..." + GuidDetails.Count;
                //            int tryCount = 0;
                //            string lastError = "";
                //            while (GuidDetails.Count > 0)
                //            {
                //                try
                //                {
                //                    var listSave = GuidDetails.Take(5);
                //                    ctx.GuidDetails.AddRange(listSave);
                //                    State = "try: " + tryCount + " Saving GuidDetails from VPS..." + GuidDetails.Count + " Last Exception : " + lastError;
                //                    ctx.SaveChanges();
                //                    GuidDetails.RemoveAll(x => listSave.Contains(x));
                //                    lastError = "";
                //                }
                //                catch (Exception e)
                //                {
                //                    tryCount++;
                //                    int i = 0;
                //                    Exception inner = e.InnerException;
                //                    string innerS = "";
                //                    while (inner != null)
                //                    {
                //                        innerS += System.Environment.NewLine + "Inner Ex:+" + i + System.Environment.NewLine + inner.Message + System.Environment.NewLine + " Inner Stack: " + (inner.StackTrace == null ? "" : inner.StackTrace);
                //                        inner = inner.InnerException;
                //                        lastError = innerS;
                //                        i++;
                //                    }
                //                }
                //            }
                //        }

                //        State = "Connection to LocalHost GuidIDs...";
                //        List<GuidIDTable> GuidIDs = new List<GuidIDTable>();
                //        using (var ctx = new AmarContext("Data Source=framesoft.ir;Integrated Security=False;Initial Catalog=FrameSoftAmar;User ID=ali;Password=hamishebahar?3773284;Connect Timeout=15;Encrypt=False;Packet Size=4096"))
                //        {
                //            State = "Selecting GuidIDs from LocalHost...";
                //            GuidIDs = (from x in ctx.GuidIDs select x).ToList();
                //        }

                //        State = "Connection to VPS GuidIDs... " + GuidIDs.Count;
                //        using (var ctx = new AmarContext("Data Source=178.32.84.95;Initial Catalog=FrameSoftAmar2;User ID=Ali;Password=hamis242he%BAHTCPASS?5848"))
                //        {
                //            //State = "Clearing GuidIDs from VPS...";
                //            //ctx.GuidIDs.RemoveRange(ctx.GuidIDs);
                //            int tryCount = 0;
                //            string lastError = "";
                //            while (GuidIDs.Count > 0)
                //            {
                //                try
                //                {
                //                    var listSave = GuidIDs.Take(5);
                //                    ctx.GuidIDs.AddRange(listSave);
                //                    State = "try: " + tryCount + " Saving GuidIDs from VPS..." + GuidIDs.Count + " Last Exception : " + lastError;
                //                    ctx.SaveChanges();
                //                    GuidIDs.RemoveAll(x => listSave.Contains(x));
                //                    lastError = "";
                //                }
                //                catch (Exception e)
                //                {
                //                    tryCount++;
                //                    int i = 0;
                //                    Exception inner = e.InnerException;
                //                    string innerS = "";
                //                    while (inner != null)
                //                    {
                //                        innerS += System.Environment.NewLine + "Inner Ex:+" + i + System.Environment.NewLine + inner.Message + System.Environment.NewLine + " Inner Stack: " + (inner.StackTrace == null ? "" : inner.StackTrace);
                //                        inner = inner.InnerException;
                //                        lastError = innerS;
                //                        i++;
                //                    }
                //                }
                //            }
                //            //State = "Inserting GuidIDs from VPS..." + GuidIDs.Count;
                //            //ctx.GuidIDs.AddRange(GuidIDs);
                //            //State = "Saving GuidIDs from VPS..." + GuidIDs.Count;
                //            //ctx.SaveChanges();
                //        }

                //        State = "Connection to LocalHost OSNames...";
                //        List<OSNameTable> OSNames = new List<OSNameTable>();
                //        using (var ctx = new AmarContext("Data Source=framesoft.ir;Integrated Security=False;Initial Catalog=FrameSoftAmar;User ID=ali;Password=hamishebahar?3773284;Connect Timeout=15;Encrypt=False;Packet Size=4096"))
                //        {
                //            State = "Selecting OSNames from LocalHost...";
                //            OSNames = (from x in ctx.OSNames select x).ToList();
                //        }

                //        State = "Connection to VPS OSNames... " + OSNames.Count;
                //        using (var ctx = new AmarContext("Data Source=178.32.84.95;Initial Catalog=FrameSoftAmar2;User ID=Ali;Password=hamis242he%BAHTCPASS?5848"))
                //        {
                //            State = "Clearing OSNames from VPS...";
                //            ctx.OSNames.RemoveRange(ctx.OSNames);
                //            State = "Inserting OSNames from VPS..." + OSNames.Count;
                //            ctx.OSNames.AddRange(OSNames);
                //            State = "Saving OSNames from VPS..." + OSNames.Count;
                //            ctx.SaveChanges();
                //        }

                //        State = "Connection to LocalHost OSVersions...";
                //        List<OSVersionTable> OSVersions = new List<OSVersionTable>();
                //        using (var ctx = new AmarContext("Data Source=framesoft.ir;Integrated Security=False;Initial Catalog=FrameSoftAmar;User ID=ali;Password=hamishebahar?3773284;Connect Timeout=15;Encrypt=False;Packet Size=4096"))
                //        {
                //            State = "Selecting OSVersions from LocalHost...";
                //            OSVersions = (from x in ctx.OSVersions select x).ToList();
                //        }

                //        State = "Connection to VPS OSVersions... " + OSVersions.Count;
                //        using (var ctx = new AmarContext("Data Source=178.32.84.95;Initial Catalog=FrameSoftAmar2;User ID=Ali;Password=hamis242he%BAHTCPASS?5848"))
                //        {
                //            State = "Clearing OSVersions from VPS...";
                //            ctx.OSVersions.RemoveRange(ctx.OSVersions);
                //            State = "Inserting OSVersions from VPS..." + OSVersions.Count;
                //            ctx.OSVersions.AddRange(OSVersions);
                //            State = "Saving OSVersions from VPS..." + OSVersions.Count;
                //            ctx.SaveChanges();
                //        }

                //        State = "Connection to LocalHost UserMessages...";
                //        List<UserMessage> UserMessages = new List<UserMessage>();
                //        using (var ctx = new AgrinContext("Data Source=framesoft.ir;Integrated Security=False;Initial Catalog=FrameSoftAmar;User ID=ali;Password=hamishebahar?3773284;Connect Timeout=15;Encrypt=False;Packet Size=4096"))
                //        {
                //            State = "Selecting UserMessages from LocalHost...";
                //            UserMessages = (from x in ctx.UserMessages select x).ToList();
                //        }

                //        State = "Connection to VPS UserMessages... " + UserMessages.Count;
                //        using (var ctx = new AgrinContext("Data Source=178.32.84.95;Initial Catalog=FrameSoftAmar2;User ID=Ali;Password=hamis242he%BAHTCPASS?5848"))
                //        {
                //            State = "Inserting UserMessages from VPS..." + UserMessages.Count;
                //            ctx.UserMessages.AddRange(UserMessages);
                //            State = "Saving UserMessages from VPS..." + UserMessages.Count;
                //            ctx.SaveChanges();
                //        }
                //        State = "Restore Complete Success";
                //    }
                //    catch (Exception e)
                //    {
                //        int i = 0;
                //        Exception inner = e.InnerException;
                //        string innerS = "";
                //        while (inner != null)
                //        {
                //            innerS += System.Environment.NewLine + "Inner Ex:+" + i + System.Environment.NewLine + inner.Message + System.Environment.NewLine + " Inner Stack: " + (inner.StackTrace == null ? "" : inner.StackTrace);
                //            inner = inner.InnerException;
                //            i++;
                //        }

                //        ErrorMessage = "State " + State + " Error: " + e.Message + " stack " + (e.StackTrace == null ? "" : e.StackTrace) + System.Environment.NewLine + " Inners :" + innerS;
                //    }
                //});
                //task.Start();


                return Content("Restore OK");
            }
            catch (Exception e)
            {
                ErrorMessage = "2 State " + State + " Error: " + e.Message + " stack " + (e.StackTrace == null ? "" : e.StackTrace);
                return Content(ErrorMessage);
            }
        }
    }
}
