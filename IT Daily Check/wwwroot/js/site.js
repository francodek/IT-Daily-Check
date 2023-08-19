// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


window.onload = function () {
    document.getElementById("download")
        .addEventListener("click", () => {
            const invoice = this.document.getElementById("detailsPage");
            console.log(invoice);
            console.log(window);
            //var opt = {
            //    margin: 5,
            //    filename: 'myfile.pdf',
            //    image: { type: 'jpeg', quality: 1 },
            //    html2canvas: { dpi: 300, letterRendering: true },
            //    jsPDF: { unit: 'mm', format: 'A4', orientation: 'landscape' }
            //};
            var opt = {
                margin: 2,
                filename: 'DailyCheck.pdf',
                image: { type: 'jpeg', quality: 0.98 },
                html2canvas: { scale: 2 },
                jsPDF: { unit: 'mm', format: 'A4', orientation: 'landscape' }
            };
            html2pdf().from(invoice).set(opt).save();
        })
}