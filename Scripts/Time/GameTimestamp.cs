using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //����ȭ�ؼ� �ν����Ϳ� ���ϼ��ְ�
public class GameTimestamp
{
    public int year;
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }
    public enum DayOfTheWeek
    {
        
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }
    public Season season;
    public int day;
    public int hour;
    public int minute;

    public GameTimestamp(int year, Season season, int day, int hour, int minute)
    {
        this.year = year;
        this.season = season;
        this.day = day;
        this.hour = hour;
        this.minute = minute;
    }

    public void UpdateClock()
    {
        minute++;

        if(minute >= 60)
        {
            minute = 0;
            hour++;
        }

        if(hour >= 24)
        {
            hour = 0;
            day++;
        }

        if(day > 30)
        {
            day = 1;
            if(season == Season.Winter)
            {
                season = Season.Spring;
                year++;
            }
            else
            {
                season++;
            }
            

        }
    }
    public DayOfTheWeek DayOfWeek
    {
        get { return GetDayOfTheWeek(); }
    }
    public DayOfTheWeek GetDayOfTheWeek() //���� �������� ���ϴ� �Լ�
    {
        //�� �ϼ� ���ϱ�
        int dayPassed = YearsToDays(year) + SeasonsToDays(season) + day;

        int dayIndex = dayPassed % 7;
        return (DayOfTheWeek)dayIndex;
    }
    public int GetPassedDay()
    {
        return YearsToDays(year) + SeasonsToDays(season) + day; //�� �ϼ�
    }

    //�ð� ������ ��ȯ
    public static int HoursToMinutes(int hour)
    {
        return hour * 60;
    }
    public static int DayToHours(int days)
    {
        return days * 24;
    }
    public static int SeasonsToDays(Season season)
    {
        int seasonIndex = (int)season;
        return seasonIndex * 30;
    }
    public static int YearsToDays(int years)
    {
        return (years-1) * 4 * 30;
    }
    // Ư�� �ð�(timestamp)�� ���� ������ Ȯ��
    public bool IsNextDay(GameTimestamp lastTime)
    {
        return year > lastTime.year ||
               (year == lastTime.year && season > lastTime.season) ||
               (year == lastTime.year && season == lastTime.season && day > lastTime.day);
    }
}
