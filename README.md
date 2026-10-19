# CodeBundler

כלי שורת פקודה (CLI) ב-C# שאורז קובצי קוד מתיקיה ומכל תתי-התיקיות שלה לתוך קובץ אחד.
שימושי למשל להגשת פרויקט לבודק כקובץ יחיד.

הכלי נכתב ב-.NET 9 עם הספריה [System.CommandLine](https://github.com/dotnet/command-line-api), ושם הפקודה שלו הוא `cb`.

## בנייה

```
dotnet build CodeBundler.sln -c Release
```

הקובץ `cb.exe` נוצר בתיקיה `CodeBundler\bin\Release\net9.0\`.
כדי להריץ את `cb` מכל מקום, מוסיפים את התיקיה הזו למשתנה הסביבה `PATH`.

## פקודות

### `bundle`: אריזת קבצים

הפקודה רצה על התיקיה הנוכחית ואורזת את כל הקבצים של השפות שנבחרו.

| אפשרות | קיצור | תיאור |
|---|---|---|
| `--language` | `-l` | **חובה.** השפות לאריזה, מופרדות ברווח, או `all` לכל השפות הנתמכות |
| `--output` | `-o` | **חובה.** שם קובץ ה-bundle או נתיב מלא אליו |
| `--note` | `-n` | הוספת הערה עם נתיב המקור לפני כל קובץ |
| `--sort` | `-s` | סדר המיון: `file-name` (ברירת מחדל, לפי שם הקובץ) או `type` (לפי סוג הקובץ) |
| `--remove-empty-lines` | `-r` | הסרת שורות ריקות מהקוד |
| `--author` | `-a` | שם יוצר הקובץ, נרשם כהערה בראש ה-bundle |

דוגמה:

```
cb bundle -l csharp -o bundle.txt -n -s type -a Ayala
```

### `create-rsp`: יצירת קובץ תגובה

הפקודה שואלת שאלות על כל אחת מהאפשרויות של `bundle` ושומרת את התשובות כקובץ תגובה (`.rsp`), כך שלא צריך להקליד את הפקודה המלאה בכל פעם:

```
cb create-rsp
cb @bundle.rsp
```

## שפות נתמכות

`csharp`/`cs`, `java`, `python`/`py`, `javascript`/`js`, `typescript`/`ts`, `html`, `css`, `c`, `cpp`, `sql`, `go`, `kotlin`, `php`, `ruby`, `swift`, `rust`

## תיקיות שלא נארזות

הכלי מדלג על תיקיות של תוצרי בנייה וכלי פיתוח:
`bin`, `obj`, `debug`, `release`, `node_modules`, `packages`, `.git`, `.vs`, `.vscode`, `.idea`, `x64`, `x86`, `dist`, `build`, `out`

## דוגמאות פלט

בתיקיה `CodeBundler/` יש שני קבצים שהכלי יצר מהקוד של הפרויקט עצמו:
- `bundle.txt`: מיון לפי שם קובץ
- `bundle-type.txt`: מיון לפי סוג קובץ
