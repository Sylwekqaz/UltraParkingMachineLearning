using LiteDB;

namespace Logic.Model
{
    public class DbContext
    {
        private readonly LiteDatabase _database;

        public DbContext(LiteDatabase database)
        {
            _database = database;
        }

        public LiteCollection<Contour> Contours => _database.GetCollection<Contour>("contours");
    }
}