function printTextValue(title, textValue) {
    var printWindow = window.open('', '', 'height=400,width=600');
    printWindow.document.write('<html><head><title>' + title + '</title>');
    printWindow.document.write('</head><body>');
    printWindow.document.write('<pre>' + textValue + '</pre>'); // Updated variable name here
    printWindow.document.write('</body></html>');
    printWindow.document.close();
    printWindow.print();
}
