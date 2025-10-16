Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

// function number_format(number, decimals, dec_point, thousands_sep) {
//   // *     example: number_format(1234.56, 2, ',', ' ');
//   // *     return: '1 234,56'
//   number = (number + '').replace(',', '').replace(' ', '');
//   var n = !isFinite(+number) ? 0 : +number,
//     prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
//     sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
//     dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
//     s = '',
//     toFixedFix = function(n, prec) {
//       var k = Math.pow(10, prec);
//       return '' + Math.round(n * k) / k;
//     };
//   // Fix for IE parseFloat(0.55).toFixed(0) = 0;
//   s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
//   if (s[0].length > 3) {
//     s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
//   }
//   if ((s[1] || '').length < prec) {
//     s[1] = s[1] || '';
//     s[1] += new Array(prec - s[1].length + 1).join('0');
//   }
//   return s.join(dec);
// }
window.addEventListener("DOMContentLoaded", () => {
  
  const ctx = document.getElementById("salaryChart");
  let salaries = JSON.parse(ctx.dataset.salaries);


new Chart(ctx, {
  type: 'line',
  data: {
    labels: salaries.map((_, i) => i),
    datasets: [{
      label: 'Salary Distribution',
      data: salaries,
      borderColor: '#4e73df',
      backgroundColor: 'rgba(78,115,223,0.1)',
      tension: 0.4, // smooth curve
      fill: true
    }]
  },
  options: {
    scales: {
      x: { display: false },
      y: {
        beginAtZero: true,
        title: { display: true, text: 'Salary' }
      }
    },
    plugins: {
      legend: { display: false }
    }
  }
});
});

window.addEventListener("DOMContentLoaded", () => {
  const ctx = document.getElementById("jobCategoryChart");
  if (!ctx) return;

  const categories = JSON.parse(ctx.dataset.categories);
  const counts = JSON.parse(ctx.dataset.counts);

  new Chart(ctx, {
    type: 'bar',
    data: {
      labels: categories,
      datasets: [{
        label: 'Jobs per Category',
        data: counts,
        backgroundColor: [
          '#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b', '#858796'
        ],
        borderWidth: 1
      }]
    },
    options: {
      scales: {
        x: {
          title: { display: true, text: 'Category' }
        },
        y: {
          beginAtZero: false, // true later
          title: { display: true, text: 'Number of Jobs' }
        }
      },
      plugins: {
        legend: { display: false },
        tooltip: { enabled: true }
      }
    }
  });
});