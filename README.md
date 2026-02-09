# 📚 Peizazhe.Epub

**Peizazhe.Epub** është një program që arkivon dhe konverton automatikisht artikujt e platformës prestigjioze ["Peizazhe të fjalës"](https://peizazhe.com/) në formatin **ePub**. Qëllimi është t'u mundësohet lexuesve t'i shijojnë këto shkrime në pajisjet e-Reader (Kindle, Kobo, etj.) ose dhe ne telefon e kompjuter por me shume pak shperqendrime.

---

### 🌐 Linqet e Projektit
* **Faqja Origjinale:** [peizazhe.com](https://peizazhe.com/)
* **Web App (Reader):** [orgeskreka.github.io/Peizazhe.Epub/](https://orgeskreka.github.io/Peizazhe.Epub/)

---

### ⚙️ Si funksionon?
Projekti është i automatizuar dhe kryen këto hapa çdo të hënë:
1.  **Skanimi:** Lexon [arkivin](https://peizazhe.com/arkivi/) e faqes për të gjetur shkrime të reja.
2.  **Konvertimi:** Për çdo artikull të ri, shkarkon përmbajtjen dhe ndërton dokumentin `.epub`.
3.  **Ruajtja:** Dokumentet e gjeneruara ruhen automatikisht në këtë depo (repository) në GitHub.
4.  **Leximi:** Faqja e frontend-it akseson skedarët direkt nga GitHub dhe i shfaq përmes një reader-i interaktiv.

---

### 🛠️ Teknologjitë & Libraritë

| Libraria | Qëllimi |
| :--- | :--- |
| **[AngleSharp](https://github.com/AngleSharp/AngleSharp)** | Parsing i përmbajtjes HTML nga faqja origjinale. |
| **[QuickEPUB](https://github.com/jonthysell/QuickEPUB)** | Gjenerimi i strukturës së dokumenteve ePub. |
| **[Bulma](https://github.com/jgthms/bulma)** | Framework-u CSS për dizajnin e pastër të faqes web. |
| **[epub.js](https://github.com/futurepress/epub.js)** | Renderimi i skedarëve ePub direkt në browser. |
| **AI (Gemini/Claude)** | Përdorur për optimizimin dhe rregullimin e kodit frontend. |

---

### TODO

- [ ] **Imazhet Lokale:** Aktualisht imazhet janë linqe HTML; duhet të shkarkohen dhe paketohen brenda skedarit ePub për lexim offline.
- [ ] **Kopertinat (Covers):** Integrimi i imazhit kryesor të artikullit si kopertinë zyrtare të librit.
- [ ] **Testimi i Pajisjeve:** Verifikimi i përputhshmërisë në modele të ndryshme si Kindle, PocketBook dhe aplikacione si Apple Books.
- [ ] **Përmirësimi i UI:** Shtimi i opsioneve për kërkim (Search) dhe kategorizim sipas autorëve në faqen web.

---
<p align="center">
  Projekt i hapur (Open Source) për dashamirësit e leximit.
</p>