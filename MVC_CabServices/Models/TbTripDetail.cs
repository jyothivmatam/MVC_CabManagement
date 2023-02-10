using System;
using System.Collections.Generic;

namespace Domain;

public partial class TbTripDetail
{
    public int TripDetailId { get; set; }

    public string? SourceAddress { get; set; }

    public string? DestinationAddress { get; set; }

    public double? Distance { get; set; }

    public long? TotalFare { get; set; }

    public DateTime? Createdate { get; set; }

    public DateTime? Updatedate { get; set; }

    public int? Pincode { get; set; }

    public int? Userid { get; set; }

}
