using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ScpDatabase;

[Table("SCPs")]
public class ScpModel : BaseModel, IImageModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; } = 0;

    [Column("number")]
    public string Number { get; set; } = "";

    [Column("class")]
    public string Class { get; set; } = "";

    [Column("procedures")]
    public string Procedures { get; set; } = "";

    [Column("description")]
    public string Description { get; set; } = "";

    [Column("status")]
    public string Status { get; set; } = "";

    [Column("username")]
    public string Username { get; set; } = "";

    [Column("image_name")]
    public string ImageName { get; set; } = "";

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}