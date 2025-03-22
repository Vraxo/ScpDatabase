using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ScpDatabase;

[Table("Personnel")]
public class PersonnelModel : BaseModel, IImageModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("age")]
    public int Age { get; set; } = 0;

    [Column("profession")]
    public string Profession { get; set; } = "";

    [Column("history")]
    public string History { get; set; } = "";

    [Column("clearance_level")]
    public int ClearanceLevel { get; set; } = 0;

    [Column("username")]
    public string Username { get; set; } = "";

    [Column("image_name")]
    public string ImageName { get; set; } = "";

    [Column("department")]
    public string Department { get; set; } = "";

    [Column("nationality")]
    public string Nationality { get; set; } = "";

    [Column("class")]
    public string Class { get; set; } = "";

    [Column("division")]
    public string Division { get; set; } = "";

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}