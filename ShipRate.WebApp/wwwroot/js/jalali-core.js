// wwwroot/js/jalali-core.js

const JalaliCore = (() => {

    // تبدیل میلادی به شمسی
    function toJalali(gYear, gMonth, gDay) {
        const g_d_m = [0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334];

        let gy = gYear - 1600;
        let gm = gMonth - 1;
        let gd = gDay - 1;

        let g_day_no = 365 * gy + Math.floor((gy + 3) / 4) - Math.floor((gy + 99) / 100) +
            Math.floor((gy + 399) / 400) + gd + g_d_m[gm];

        if (gm > 1 && ((gy % 4 === 0 && gy % 100 !== 0) || (gy % 400 === 0))) {
            g_day_no++;
        }

        let j_day_no = g_day_no - 79;
        let j_np = Math.floor(j_day_no / 12053);
        j_day_no %= 12053;

        let jy = 979 + 33 * j_np + 4 * Math.floor(j_day_no / 1461);
        j_day_no %= 1461;

        if (j_day_no >= 366) {
            jy += Math.floor((j_day_no - 1) / 365);
            j_day_no = (j_day_no - 1) % 365;
        }

        let jm, jd;
        if (j_day_no < 186) {
            jm = 1 + Math.floor(j_day_no / 31);
            jd = 1 + (j_day_no % 31);
        } else {
            jm = 7 + Math.floor((j_day_no - 186) / 30);
            jd = 1 + ((j_day_no - 186) % 30);
        }

        return { year: jy, month: jm, day: jd };
    }

    // تبدیل شمسی به میلادی
    function toGregorian(jYear, jMonth, jDay) {
        const sal_a = [0, 31, 62, 93, 124, 155, 186, 216, 246, 276, 306, 336];

        let jy = jYear - 979;
        let jm = jMonth - 1;
        let jd = jDay - 1;

        let j_day_no = 365 * jy + Math.floor(jy / 33) * 8 +
            Math.floor((jy % 33 + 3) / 4) + jd + sal_a[jm];

        let g_day_no = j_day_no + 79;
        let gy = 1600 + 400 * Math.floor(g_