namespace Emgu.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class EmguDBContext : DbContext
    {
        // Your context has been configured to use a 'FacesDB' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Emgu.Models.FacesDB' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'FacesDB' 
        // connection string in the application configuration file.
        public EmguDBContext()
            : base("name=Emgu")
        {
        }
        public virtual DbSet<Faces> Faces { get; set; }
    }

    // Add a DbSet for each entity type that you want to include in your model. For more information 
    // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.



    public class Faces
    {
        public int id { get; set; }
        public int userid { get; set; }
        public string username { get; set; }
        public byte[] faceSample { get; set; }

    }
}