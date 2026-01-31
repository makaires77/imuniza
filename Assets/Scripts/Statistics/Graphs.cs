using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graphs : MonoBehaviour
{
    [SerializeField] private MarkEvent markEventPrefab = null;
    [SerializeField] private Transform contentMarks = null;

    [Space]
    [SerializeField] private ColumnDay columnDayPrefab = null;
    [SerializeField] private Transform contentDays = null;

    Dictionary<int, ColumnDay> eventsInDays = new Dictionary<int, ColumnDay>();

    private List<MarkEvent> marks = new List<MarkEvent>();

    public void AddValue(int day)
    {
        if (eventsInDays.ContainsKey(day))
            eventsInDays[day].ValueInDay++;
        else
        {
            ColumnDay columnDay = Instantiate(columnDayPrefab, contentDays);
            columnDay.ValueInDay = 1;
            columnDay.Day = day;

            eventsInDays.Add(day, columnDay);
            columnDay.gameObject.SetActive(true);
        }

        MarksUpdate();
    }

    private void MarksUpdate()
    {
        foreach (var mark in marks)
            Destroy(mark.gameObject);

        marks.Clear();

        int maxValueInDay = 0;
        foreach (var dayEvents in eventsInDays)
        {
            if (dayEvents.Value.ValueInDay > maxValueInDay)
                maxValueInDay = dayEvents.Value.ValueInDay;
        }

        if (maxValueInDay == 1)
        {
            MarkEvent markEvent = Instantiate(markEventPrefab, contentMarks);
            markEvent.NumberEvents = maxValueInDay;

            markEvent.transform.SetSiblingIndex(markEvent.transform.GetSiblingIndex() - 1);
            markEvent.gameObject.SetActive(true);

            marks.Add(markEvent);
        }
        else
        {
            for (int i = maxValueInDay; i > 1; i /= 2)
            {
                MarkEvent markEvent = Instantiate(markEventPrefab, contentMarks);
                markEvent.NumberEvents = i;

                markEvent.transform.SetSiblingIndex(markEvent.transform.GetSiblingIndex() - 1);
                markEvent.gameObject.SetActive(true);

                marks.Add(markEvent);
            }
        }

        foreach (var colums in eventsInDays)
            colums.Value.SetHeightBar(maxValueInDay);
    }
}
