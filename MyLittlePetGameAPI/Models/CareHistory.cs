using System;
using System.Collections.Generic;

namespace MyLittlePetGameAPI.Models;

public partial class CareHistory
{
    public int CareHistoryId { get; set; }

    public int PlayerPetId { get; set; }

    public int PlayerId { get; set; }

    public int ActivityId { get; set; }

    public DateTime? PerformedAt { get; set; }

    public virtual CareActivity Activity { get; set; } = null!;

    public virtual User Player { get; set; } = null!;

    public virtual PlayerPet PlayerPet { get; set; } = null!;
}
