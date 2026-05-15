const fs = require('fs');

const content = fs.readFileSync('C:/Moon/Vault/index.html', 'utf8');
const start = content.indexOf('\n<script>\nfunction pageApp() {');
const afterEnd = content.indexOf('\n</script>\n</body>') + '\n</script>'.length;

const before = content.slice(0, start);
const after  = content.slice(afterEnd);

const newScript = fs.readFileSync('C:/Moon/Vault/new_pageapp.js', 'utf8');

const newContent = before + '\n' + newScript + after;
fs.writeFileSync('C:/Moon/Vault/index.html', newContent, 'utf8');
console.log('Done. File size:', newContent.length);
