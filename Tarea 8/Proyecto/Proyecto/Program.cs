using System;
using System.Windows.Forms;
using Proyecto.Data;
using Proyecto.Forms;

namespace Proyecto
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                InicializadorBD.Iniciar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible iniciar la base de datos local.\n\n" + ex.Message,
                    "Proyecto Transporte", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new FormPrincipal());
        }
    }
}