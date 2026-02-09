# Peizazhe.Epub
Program qe kthen gjithe artikujt e faqes "Peizazhe të fjalës" ne formatin epub ne menyre qe te mund te lexohen dhe nga pajisjet E-Reader
--- 
Linku i faqes origjinale: https://peizazhe.com/
Linku i faqes: https://orgeskreka.github.io/Peizazhe.Epub/
---
Projekti lexon cdo te hene gjithe artikujt nga [arkivi](https://peizazhe.com/arkivi/) i faqes dhe cdo artikull te ri ( qe nuk eshte i shkarkuar ) e lexon, nderton dokumentin .epub dhe me pas e ruan ne Github.
Faqja e Web-it lexon nga Github-i gjithe artikujt ( me prapashtesen .epub ) te shkarkuar dhe i shfaq.
---
# Librarite e perdorura:
[AngleSharp](https://github.com/AngleSharp/AngleSharp) per leximin e permbajtjes se faqeve html
[QuickEPUB](https://github.com/jonthysell/QuickEPUB) per te gjeneruar dokumentin epub
[Bulma](https://github.com/jgthms/bulma) per ndertimin e faqes web
[epub.js](https://github.com/futurepress/epub.js) per te shfaqur dokumentet epub ne web
ClaudeCode/Google Gemini per te gjeneruar/rregulluar kodin e frontendit.
---
# TODO:
- [] Aktualisht imazhet brenda artikujve jane referuar si linqe html, duhet gjetur nje menyre qe te ngarkohen lokalisht.
- [] Tek website origjinal cdo artikull ka nje imazh, do ishte bukur sikur te merrej dhe ai imazh dhe te perdorej si cover per dokumentin .epub
- [] Dokumentat e gjeneruar duhet te testohen ne pajisje dhe viewer te ndryshem per te pare sa funksionale jane.