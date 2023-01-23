using System.Data.SQLite;

namespace AtHome.ControleDeEstoque.Data
{
    public static class DbConn
    {
        public static SQLiteConnection Connection { get; set; }

    }
}
