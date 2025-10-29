Volunteer Management System:
____________________________
Overview:
A Volunteer Management System for managing volunteers and requests in an organization. Volunteers can select requests, update their status, and administrators can manage both volunteers and requests.

Features:
Volunteer can pick one request at a time and report completion or cancellation
Administrator can manage requests and volunteers
System maintains a separate system clock for time simulation
Request history is tracked for auditing
Volunteers can filter requests by distance

Roles:
Administrator: 
manages volunteers and requests, can also act as a volunteer
Volunteer: 
selects requests to handle, updates status, sets personal preferences

Usage:
Administrator logs in and creates requests with maximum completion times
Volunteer logs in, filters requests by distance, and selects a request
Volunteer completes or cancels the request
Administrator monitors request statuses

Architecture:
Layered architecture (UI, presentation, business logic, data)
Time simulation via a separate system clock
History of assignments maintained for auditing

Example Workflow:
Admin creates a food delivery request
Volunteer sees the request and accepts it
Volunteer completes the request
Admin checks request completion




מערכת ניהול מתנדבים:
                                                                                                                                              ________________________
תיאור כללי:
מערכת ניהול מתנדבים לניהול מתנדבים וקריאות בארגון. המערכת מאפשרת למתנדבים לבחור קריאות לטיפול ולעדכן את מצבן, בעוד שהמנהל יכול לנהל מאגרי מתנדבים וקריאות.

פיצ'רים:
מתנדב יכול לבחור קריאה אחת בלבד בזמן נתון ולדווח על סיומה או ביטולה
מנהל יכול לנהל מתנדבים וקריאות, ויכול גם לפעול כמתנדב רגיל
המערכת משתמשת בשעון מערכת נפרד לצורך הדמיית זמן
שמירת היסטוריית הקצאות לצורך בקרה וניהול
מתנדבים יכולים לסנן קריאות לפי מרחק מרבי

תפקידים:
מנהל: 
מנהל את הקריאות והמתנדבים, יכול גם לבחור קריאות כמתנדב רגיל
מתנדב: 
בוחר קריאות, מדווח על מצב הקריאה, ומגדיר העדפות אישיות

שימוש:
המנהל נכנס למערכת ויוצר קריאות עם זמן מקסימלי לסיום
המתנדב נכנס, מסנן קריאות לפי מרחק, ובוחר קריאה לטיפול
המתנדב מסיים את הקריאה או מבטל אותה
המנהל עוקב אחרי סטטוס הקריאות

ארכיטקטורה:
מבוססת ארכיטקטורת שכבות (UI,Presentation, Business Logic, Data)
שעון מערכת נפרד להדמיית זמן
שמירת היסטוריית הקצאות לקריאות

דוגמת זרימת עבודה:
המנהל יוצר קריאה למשלוח מזון
מתנדב רואה את הקריאה ובוחר לטפל בה
המתנדב מסיים את הטיפול בקריאה
המנהל בודק שהקריאה הושלמה
