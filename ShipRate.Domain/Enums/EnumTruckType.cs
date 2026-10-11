namespace ShipRate.Domain.Enums
{
    public enum EnumTruckType
    {
        RigidTruck = 1,
        SemiTrailer = 2,
        Flatbed = 3,
        BoxTrailer = 4,
        Reefer = 5,
        Tanker = 6,
        LowBed = 7,
        ContainerChassis = 8,
        CarCarrier = 9,
        Tipper = 10,
        Hazmat = 11,
        Pickup = 12
    }
}



//نوع ماشین 	توضیح کاربردی 	ظرفیت معمول (وزن ناخالص) 	ویژگی‌ها
//کامیون (Truck – Rigid) 	حمل بار داخلی یا شهری 	3–10 تن 	اتاق ثابت یا کانتینردار ثابت
//تریلر (Trailer / Semi‑Trailer) 	بین‌المللی، ترانزیت و حمل سنگین 	20–25 تن 	متصل به کشنده (Tractor)
//کفی (Flatbed) 	بارهای پالت، رول، فولاد 	22–24 تن 	بدون دیواره
//مسقف (Curtain Sider / Box) 	بارهای حساس به آب و گردوغبار 	20–22 تن 	پوشش برزنت یا فلز
//یخچالی (Reefer) 	مواد فاسدشدنی / دارو / مواد غذایی 	18–20 تن 	دارای سیستم سرمایش
//2️⃣ ماشین‌های خاص (Specialized Equipment)
//نوع ماشین 	کاربرد تخصصی 	ویژگی تکنیکی
//تانکر (Tank Trailer) 	حمل مایعات سوختی، شیمیایی 	استیل / آلومینیوم، ضد نشت
//کمرشکن (Low‑Bed / Heavy Duty) 	حمل ماشین‌آلات سنگین، پروژه‌ای 	ظرفیت 40–100 تن، ارتفاع کم
//تیغه‌دار (Tipping Trailer) 	حمل مصالح ساختمانی، فله‌ای 	تخلیه از عقب
//کامیون چادری (Tautliner) 	بارهای عمومی و حمل پالت 	دارای چادر قابل باز شدن
//کامیون کانتینربر (Container Carrier / Chassis) 	حمل کانتینر 20 ft / 40 ft 	قفل Twist‑Lock دارد
//ماشین حمل خودرو (Car Carrier) 	حمل چند خودرو همزمان 	دو طبقه، هیدرولیک
//تانکر گازی (LPG / LNG Tanker) 	گاز مایع یا طبیعی 	فشار بالا / عایق‌ دما پایین
//ماشین ADR (Hazmat Truck) 	مواد خطرناک 	دارای تابلوی Orange Plate و گواهی ADR
//3️⃣ ماشین‌های پشتیبان و لجستیکی (Support Fleet)
//نام 	کاربرد
//وانت / کامیونت (Pickup / Light Truck) 	حمل نمونه بار، سند یا محموله‌های سبک