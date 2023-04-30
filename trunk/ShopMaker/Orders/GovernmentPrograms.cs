using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MakerShop.Orders
{
    /// <summary>
    /// Enumeration that represents the type of a GovernmentPrograms
    /// </summary>
    public enum GovernmentPrograms : short
    {

        Medicaid_Not_Medicare = 1,
        Supplemental_Nutrition_Assistance_Program_Food_Stamp = 2,
        Temporary_Assistance_to_Needy_Families_TANF = 3,
        Supplemental_Security_Income_SSI_Not_the_same_as_Social_Security_Benefits = 4,
        National_School_Lunch_Program_NSL = 5,
        LowIncome_Home_Energy_Assistance_Program_LIHEAP = 6,
        Federal_Public_Housing_Assistance_Section_8 = 7,
        Bureau_of_Indian_Affairs_Programs_Tribal_Temporary_Assistance_for_Needy_Families = 8


    }
}
