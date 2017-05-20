namespace Agrin.UI.Update
{
    public static class Main
    {
        public static void Load()
        {
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                try
                {
                    string fileName = Agrin.IO.Helper.MPath.CurrentAppDirectory + "\\update.upt";
                    string appFile = Agrin.IO.Helper.MPath.CurrentAppDirectory + "\\Agrin.UI.exe";
                    if (System.IO.File.Exists(fileName))
                        return;
                    //System.Reflection.Assembly info = new (appFile);
                    //info.
                    System.IO.File.Create(fileName).Dispose();
                    Agrin.UI.Update.MainWindow window = new Agrin.UI.Update.MainWindow();
                    window.ShowDialog();
                    System.IO.File.Delete(fileName);
                }
                catch
                {

                }
            });
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
        }
    }
}
