using System;

public class Player: BaseEntity
{
	public int ShirtNumber { get; set; }
	public string Name { get; set; }
	public string Surname { get; set; }
	public int TeamId { get; set; }
    public Team Team { get; set; }
	public int Goals { get; set; } = 0;
}
