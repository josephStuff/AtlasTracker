namespace AtlasTracker.Models
{
    public class Project
    {
        // ------------- PRIMARY KEY ---------------------- <
        public int Id { get; set; }




        // ---------------- NAME ------------------------------- <
        public string? CompanyName { get; set; }


        // ----------------- DESCRIPTION ------------------- <
        public string? ProjectDescription { get; set; }


        // -------------------- CREATED -------------------- <
        public DateTimeKind? Created { get; set; }


        // --------------------- START DATE -------------------- <
        public DateOnly? ProjectStarted { get; set; }


        // ----------------------------- END DATE ------------------- <
        public DateOnly? ProjectEnded { get; set; }


        // ------------- COMPANY ID -------------------------------- <
        //public virtual[] CompanyCompanyId { get; set; }

        // --------------------- PROJECT PRIORITY --------------------- <
        //public virtual? Priority { get; set; }

    
        public byte[]? ImageData { get; set; } = Array.Empty<byte>();
        public string? ImageExt { get; set; } = "";


        // ---------------------- ARCHIVED ---------------------------- <

        //  -------------------------- MEMBER (S) ----------------------- <

        // ------------------------------ TICKET (S) --------------------- <






    }
}
