window.renderDashboardCharts = function (monthlyLabels, monthlyCounts, activePercent, inactivePercent, alumniPercent, satLabels, satPresent, satAbsent, satExcused) {
    if (typeof Chart === 'undefined') {
        return;
    }

    const isDark = document.documentElement.classList.contains('dark');
    const textColor = isDark ? '#cbd5e1' : '#64748b';
    const gridColor = isDark ? '#1e293b' : '#f1f5f9';

    try {
        // Monthly Attendance Chart
        const monthlyCanvas = document.getElementById('monthlyChart') || document.getElementById('monthlyAttendanceChart');
        if (monthlyCanvas) {
            const monthlyCtx = monthlyCanvas.getContext('2d');
            if (window.monthlyChartInstance) {
                window.monthlyChartInstance.destroy();
                window.monthlyChartInstance = null;
            }
            window.monthlyChartInstance = new Chart(monthlyCtx, {
                type: 'bar',
                data: {
                    labels: monthlyLabels,
                    datasets: [{
                        label: 'Present',
                        data: monthlyCounts,
                        backgroundColor: isDark ? 'rgba(56, 189, 248, 0.85)' : 'rgba(36, 107, 156, 0.85)',
                        borderRadius: 6,
                        barPercentage: 0.6,
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    resizeDelay: 150,
                    plugins: { legend: { display: false } },
                    scales: {
                        y: { beginAtZero: true, grid: { color: gridColor }, ticks: { color: textColor } },
                        x: { grid: { display: false }, ticks: { color: textColor } }
                    }
                }
            });
        }

        // Status Distribution Chart
        const statusCanvas = document.getElementById('statusChart') || document.getElementById('statusDistributionChart');
        if (statusCanvas) {
            const statusCtx = statusCanvas.getContext('2d');
            if (window.statusChartInstance) {
                window.statusChartInstance.destroy();
                window.statusChartInstance = null;
            }
            window.statusChartInstance = new Chart(statusCtx, {
                type: 'doughnut',
                data: {
                    labels: ['Active', 'Inactive', 'Alumni'],
                    datasets: [{
                        data: [activePercent, inactivePercent, alumniPercent],
                        backgroundColor: ['#3b82f6', '#f59e0b', '#94a3b8'],
                        borderWidth: isDark ? 2 : 0,
                        borderColor: isDark ? '#1e293b' : '#ffffff'
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    resizeDelay: 150,
                    cutout: '70%',
                    plugins: {
                        legend: {
                            display: false
                        }
                    }
                }
            });
        }

        // Current Month Saturday Attendance Chart
        const currentMonthCanvas = document.getElementById('currentMonthChart');
        if (currentMonthCanvas && satLabels) {
            const currentMonthCtx = currentMonthCanvas.getContext('2d');
            if (window.currentMonthChartInstance) {
                window.currentMonthChartInstance.destroy();
                window.currentMonthChartInstance = null;
            }
            window.currentMonthChartInstance = new Chart(currentMonthCtx, {
                type: 'bar',
                data: {
                    labels: satLabels,
                    datasets: [
                        {
                            label: 'Present',
                            data: satPresent,
                            backgroundColor: '#22c55e',
                            borderRadius: 4,
                        },
                        {
                            label: 'Excused',
                            data: satExcused,
                            backgroundColor: '#f59e0b',
                            borderRadius: 4,
                        },
                        {
                            label: 'Absent',
                            data: satAbsent,
                            backgroundColor: '#ef4444',
                            borderRadius: 4,
                        }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    resizeDelay: 150,
                    plugins: {
                        legend: { display: true, position: 'bottom', labels: { color: textColor } }
                    },
                    scales: {
                        x: { stacked: true, grid: { display: false }, ticks: { color: textColor } },
                        y: { stacked: true, beginAtZero: true, grid: { color: gridColor }, ticks: { color: textColor, stepSize: 1 } }
                    }
                }
            });
        }
    } catch (e) {
        console.warn('Dashboard Chart Error:', e);
    }
};

window.exportToPdf = function(dateStr) {
    const uiContent = document.getElementById('ui-content');
    const pdfContent = document.getElementById('pdf-export-content');
    
    if (!uiContent || !pdfContent) {
        alert("Cannot find content to export.");
        return;
    }
    
    uiContent.style.display = 'none';
    pdfContent.style.display = 'block';

    const opt = {
        margin:      0.4,
        filename:    'KOA_Attendance_' + dateStr + '.pdf',
        image:       { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 2, useCORS: true, logging: false },
        jsPDF:       { unit: 'in', format: 'letter', orientation: 'portrait' }
    };

    if (typeof html2pdf === 'undefined') {
        alert("html2pdf library is not loaded.");
        pdfContent.style.display = 'none';
        uiContent.style.display = 'block';
        return;
    }

    html2pdf().set(opt).from(pdfContent).save().then(() => {
        pdfContent.style.display = 'none';
        uiContent.style.display = 'block';
    }).catch(err => {
        console.error(err);
        pdfContent.style.display = 'none';
        uiContent.style.display = 'block';
        alert("Error exporting PDF. Check console.");
    });
};