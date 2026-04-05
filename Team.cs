using System;

public class Team :BaseEntity
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int StadiumId { get; set; }
    public Stadium? Stadium { get; set; }
}
