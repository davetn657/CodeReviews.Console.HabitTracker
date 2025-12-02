# Console Habit Tracker

Habit tracking C# application.
Console based CRUD application to track habits using C# and SQLite

## Technologies Used
- C#
- SQLite

## Given Requirements
- On application start, it should create a sqlite databasem if one isn't present
- It should create multiple tables in the database and it should fill in the table with some records
- Users should be able to create their own habit trackers
- Users should be able to insert, edit, and delete table data
- Users should be able to view a summary of the year for their chosen habit
- It should handle all possible errors so the application never crashes

## Features
- SQLite database connection
	-
- A console based UI
- CRUD DB functions
- Basic yearly summaries

## Challenges
- First time using SQLite so most of the challenges I faced was due to my limited knowledge on how sql worked in a c# project
- When creating the summary report function I ran into a problem, the dates that was stored in the table followed the format of 'yyyy-mm-dd' but I wanted the user to be able to filter their report based on the year. I had some trouble with this however, i resolved it by using the strftime method which allowed me to use only the year for queries
- As I was going through the project guidlines, I found that I had to change alot of my defined functions as they didn't really account for different inputs such as different tables or units of measurements. 

## Lessons Learned
- I found myself having to rewrite alot of my code to account for multiple different tables and units of measurment so I feel for the next project I need to better plan out my functions
- Create a map of features that I want to include in the project and link common functions that would be used between features. This would allow me to follow DRT principles closely and help with spaghetti code.

## Areas to Improve
- Learn more about visual studio shortcuts, I found myself needing to rename or change variables and found that instead of typing each new change, visual studio offers a large selection of shortcuts such as the refactor tool
- Reduce spaghetti code by utilizing classes.
- Learn more about SQL queries, I needed to constantly lookup queries.
