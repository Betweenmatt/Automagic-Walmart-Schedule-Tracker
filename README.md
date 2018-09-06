# Automagic-Walmart-Schedule-Tracker

The main operations performed by this app are conducted in the `SiteScraper` class and the `CalManager` class. `SiteScraper` has only one entrance, `Execute()`, which uses a `HttpClient` to login to WalmartOne and returns the schedule html for `HtmlAgilityPack` to parse. The `Execute()` method returns a `SiteScraperReturnObject` that indicates the return status as well as any data( a list of `Day` objects).

The list that is returned by `SiteScraper` is then sent to the `CalManager` class via `Execute()` which also requires a `SettingsObject`. The `SettingsObject` holds all the settings for the app such as event colors, event title, etc.

The `CalManager` class also contains a few helper methods, such as `ChangeTimeslot()`: for changing a specific timeslot; `GetCalendars()`: for getting a list of all calendars on the device; `GetEventCollection()`: for retrieving all the events stored in the database; and `DeleteTimeslot()`: for deleting the given timeslot.

The app uses `JobScheduler` for running as a service, which activates immediately on toggling, followed by once per 3 hours. The job scheduler actually runs two services, the `EventService` and the `ReminderService`. `EventService` is the service which scrapes WalmartOne and adds/updates your calendar, `ReminderService` follows about 5 minutes later which updates all the events reminders. This route was needed because there is a race condition when using `CalendarContract` to add an event and then edit the reminder. If anyone has any other solutions feel free to let me know!

If you inspect the `Day` class, you can see that there are two DateTime properties for start and end. The `BackupX` property is set when the object is created or updated by the `SiteScraper` data, and is what is compared when checking the schedule in future transactions. The start/end properties without the `Backup` prefix are the displayed data. When a user updates a specific timeslot, only the start/end properties are adjusted preventing overwrite unless the shift is updated on WalmartOne.

Thanks for using my app! If you have any questions or concerns you can create an issue, or comment on the Reddit post!
