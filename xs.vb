Imports Softone

Module xs
    Public dayoneApiURL = "https://api.day-one.gr/proxy"
    Public dayoneRenewURL = "https://dayone.oncloud.gr/s1services/JS/ModuleCheck.Couriers/renew"
    Public XXX As XSupport

    Public CourierNames As New Dictionary(Of Integer, String) From {
        {1, "ACS"},
        {2, "Γενική Ταχυδρομική"},
        {3, "Speedex"},
        {4, "Box Now"},
        {5, "Ταχυδέμα"},
        {6, "Courier Center"},
        {7, "ΕΛΤΑ Courier"}
    }

    Public CourierCodes As New Dictionary(Of Integer, String) From {
       {651, "1"},
       {652, "2"},
       {653, "3"},
       {650, "4"},
       {655, "5"},
       {654, "6"},
       {657, "7"}
   }

    Public CourierCodesInverted As New Dictionary(Of Integer, Integer) From {
       {1, 651},
       {2, 652},
       {3, 653},
       {4, 650},
       {5, 655},
       {6, 654},
       {7, 657}
    }
End Module
