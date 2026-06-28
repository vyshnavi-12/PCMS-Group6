public class PhysicianWorkload
{
    public int PastMorningAssignments { get; set; }

    public int PastNightAssignments { get; set; }

    public int CurrentMorningAssignments { get; set; }

    public int CurrentNightAssignments { get; set; }

    public int TotalAssignments =>
        PastMorningAssignments +
        PastNightAssignments +
        CurrentMorningAssignments +
        CurrentNightAssignments;

    public int TotalNightAssignments =>
        PastNightAssignments +
        CurrentNightAssignments;
}