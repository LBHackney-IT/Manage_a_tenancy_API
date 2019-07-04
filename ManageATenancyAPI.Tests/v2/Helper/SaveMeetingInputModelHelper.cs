﻿using System;
using System.Collections.Generic;
using System.Text;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.Tests.v2.Helper
{
    public static class SaveMeetingInputModelHelper
    {
        public static SaveETRAMeetingInputModel Create()
        {
            return new SaveETRAMeetingInputModel
            {
                MeetingAttendance = new MeetingAttendees
                {
                    NumberOfAttendees = 1
                },
                Issues = new List<MeetingIssue>
                {
                    new MeetingIssue
                    {
                        IssueTypeId = "100000501",
                        IssueLocationName = "De Beauvoir Estate  1-126 Fermain Court",
                        IssueNote = "Bad things have happened please fix"
                    },
                    new MeetingIssue
                    {
                        IssueTypeId = "100000501",
                        IssueLocationName = "De Beauvoir Estate  1-126 Fermain Court",
                        IssueNote = "Bad things have happened please fix 2"
                    }
                },
                SignOff = new SignOff
                {
                    Name = "Jeff Pinkham",
                    Role = "chair",
                    Signature = "iVBORw0KGgoAAAANSUhEUgAABxAAAAEsCAYAAADjByaAAAAgAElEQVR4Xu3dAcx0V1of9n9KCSRhsZ2UgtgS2wkVC6LYbiqFaAO2qzRUdJFtQZuyINkmUSBSVdsVqKlE9dlJVVGxkm1FaqAo2K5CdiuF2A6i3TaVbDcbZVHTfHaqUKgaeU2yFaVN7d2FLCRLXT27M1/enW/uvefO3Jn33vv+rvTq2933zrnn/M6ZWWn+73PO74iLAAECBAgQIECAAAECBAgQIECAAAECBAgQIECAAAECG4HfQYIAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQJbAQGitUCAAAECBAgQIECAAAECBAgQIECAAAECBAgQIECAwA0BAaLFQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQICAANEaIECAAAECBAgQIECAAAECBAgQIECAAAECBAgQIEDgZgEViFYFAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQI3BASIFgMBAgQIECBAgAABAgQIECBAgAABAgQIECBAgAABAgJEa4AAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAgZsFVCBaFQQIECBAgAABAgQIECBAgAABAgQIECBAgAABAgQI3BAQIFoMBAgQIECAAAECBAgQIECAAAECBAgQIECAAAECBAgIEK0BAgQIECBAgAABAgQIECBAgAABAgQIECBAgAABAgRuFlCBaFUQIECAAAECBAgQIECAAAECBAgQIECAAAECBAgQIHBDQIBoMRAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgIEC0BggQIECAAAECBAgQIECAAAECBAgQIECAAAECBAgQuFlABaJVQYAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIDADQEBosVAgAABAgQIECBAgAABAgQIECBAgAABAgQIECBAgIAA0RogQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQOBmARWIVgUBAgQIECBAgAABAgQIECBAgAABAgQIECBAgAABAjcEBIgWAwECBAgQIECAAAECBAgQIECAAAECBAgQIECAAAECAkRrgAABAgQIECBAgAABAgQIECBAgAABAgQIECBAgACBmwVUIFoVBAgQIECAAAECBAgQIECAAAECBAgQIECAAAECBAjcEBAgWgwECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECAgQrQECBAgQIECAAAECBAgQIECAAAECBAgQIECAAAECBG4WUIFoVRAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgcENAgGgxECBAgAABAgQIECBAgAABAgQIECBAgAABAgQIECAgQLQGCBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBC4WUAFolVBgAABAgQIECBAgAABAgQIECBAgAABAgQIECBAgMANAQGixUCAAAECBAgQIECAAAECBAgQIECAAAECBAgQIECAgADRGiBAgAABAgQIECBAgAABAgQIECBAgAABAgQIECBA4GYBFYhWBQECBAgQIECAAAECBAgQIECAAAECBAgQIECAAAECNwQEiBYDAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQKrDRDvS3LXZnSvJ3nNXBMgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAg0C6wlgrEW5P8rSTftDP0X0zy/iTvtJO4kwABAgQIECBAgAABAgQIECBAgAABAgQIECBAgMDVFVhLgPhXk3x3xzT+bJLvubpTbOQECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIE2gXWEiD+VpLf2THs+t2Xt5O4kwABAgQIECBAgAABAgQIECBAgAABAgQIECBAgMDVFVhLgPjuwBSuZZxXd6UaOQECBAgQIECAAAECBAgQIECAAAECBAgQIECAwFkE1hKsCRDPslw8hAABAgQIECBAgAABAgQIECBAgAABAgQIECBAYO0CAsS1z7DxESBAgAABAgQIECBAgAABAgQIECBAgAABAgQIEBghIEAcgeVWAgQIECBAgAABAgQIECBAgAABAgQIECBAgAABAmsXECCufYaNjwABAgQIECBAgAABAgQIECBAgAABAgQIECBAgMAIAQHiCCy3EiBAgAABAgQIECBAgAABAgQIECBAgAABAgQIEFi7wBoCxDuSvDkwUbcleWftk2l8BAgQIECAAAECBAgQIECAAAECBAgQIECAAAECBI4VWEOAeGuStwWIxy4FrydAgAABAgQIECBAgAABAgQIECBAgAABAgQIECCQCBCtAgIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIEbgisIUC8L8krA3N6f5JXzTsBAgQIECBAgAABAgQIECBAgAABAgQIECBAgAABAv0CawgQvyXJGwMTfVeSv2cxECBAgAABAgQIECBAgAABAgQIECBAgAABAgQIECCw/gBRBaJVToAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQGAigTVUID6Z5NqAx1NJ6j4XAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQI9AmsIEFUgWuIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIEJhIQIE4EqRkCBAgQIECAAAECBAgQIECAAAECBAgQIECAAAECaxBYQ4BoC9M1rERjIECAAAECBAgQIECAAAECBAgQIECAAAECBAgQmIXAGgJEW5jOYinpBAECBAgQIECAAAECBAgQIECAAAECBAgQIECAwBoE1hAgPpLkuYHJeDTJ82uYMGMgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgcEoBAeIpdbVNgAABAgQIECBAgAABAgQIECBAgAABAgQIECBAYGECawgQbWG6sEWnuwQIECBAgAABAgQIECBAgAABAgQIECBAgAABAvMVWEOA+GSSawPETyWp+1wECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECPQIrCFAVIFoiRMgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBCYSGANAeLjSZ4e8HgiyTMTmWmGAAECBAgQIECAAAECBAgQIECAAAECBAgQIECAwGoF1hAgqkBc7fI0MAIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAgXMLrCFAVIF47lXjeQQIECBAgAABAgQIECBAgAABAgQIECBAgAABAqsVWEOAqAJxtcvTwAgQIECAAAECBAgQIECAAAECBAgQIECAAAECBM4tsIYAUQXiuVeN5xEgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECKxWYA0BogrE1S5PAyNAgAABAgQIECBAgAABAgQIECBAgAABAgQIEDi3wBoCxJYKxEeTPH9uXM8jQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgsDSBNQSIDyZ5cQD+oSQvLW1y9JcAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIDAuQUEiOcW9zwCBAgQIECAAAECBAgQIECAAAECBAgQIECAAAECMxa4KgHi/UlenfE86BoBAgQIECBAgAABAgQIECBAgAABAgQIECBAgACBWQisIUC8L8krA5oCxFksN50gQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBCYu4AAce4zpH8ECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIEziggQDwjtkcRIECAAAECBAgQIECAAAECBAgQIECAAAECBAgQmLuAAHHuM6R/BAgQIECAAAECBAgQIECAAAECBAgQIECAAAECBM4oIEA8I7ZHESBAgAABAgQIECBAgAABAgQIECBAgAABAgQIEJi7gABx7jOkfwQIECBAgAABAgQIECBAgAABAgQIECBAgAABAgTOKLCGAPHWJG8PmN2W5J0zunoUAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAgUUKrCFAvCPJmwP6dyb5xCJnSKcJECBAgAABAgQIECBAgAABAgQIECBAgAABAgQInFFgDQFicb07YLaWcZ5xaXgUAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIDAVRRYS7AmQLyKq9eYCRAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIEJhcQIE5OqkECBAgQIECAAAECBAgQIECAAAECBAgQIECAAAECyxVYS4D4uSRf0jENn0py63KnSM8JECBAgAABAgQIECBAgAABAgQIECBAgAABAgQInE9gLQGiLUzPt2Y8iQABAgQIECBAgAABAgQIECBAgAABAgQIECBAYMUCawgQq7rw7Z45ei3JfSuew66hfV+SDyX5ms0Nv5LksSQvXUELQyZAgAABAgQIECBAgAABAgQIECBAgAABAgQIEGgUWEOAWOHgKwLELxL4ziQ/32FyZ5JPNK4PtxEgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECFwxgTUEiI8keU6A+EUCv5TkGzpMnkry5BVb54ZLgAABAgQIECBAgAABAgQIECBAgAABAgQIECDQKLCGALHCsGsCxBsCtaXrryX50g6Tl5M82Lg+3EaAAAECBAgQIECAAAECBAgQIECAAAECBAgQIHDFBNYQID6zOduva+peSFJVilflejzJ0z2D/XCSD14VDOMkQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAYJ7CGAPH5JA/3DPuqbdn515N8V4/HE0kqdHURIECAAAECBAgQIECAAAECBAgQIECAAAECBAgQuElgDQHiq0nuFSB+XuDuJNcH1vn9ScrMRYAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQOAmgasQIF6lirv7krzSs84/meRf8T4gQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAg0CWwhgDxE0lu75niq7SF6etJ7uqxeC1JhYwuAgQIECBAgAABAgQIECBAgAABAgQIECBAgAABAnsF1hAgvjswt48mqXMS1361bF96lcLUtc+38REgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIETiKwhgDx00ne06Pz7yT5b0+iN69GH0zy4kCXrtJ2rvOaHb0hQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECCxEYA0B4lAF4v1JXl3IfBzazVuTXE9yx0ADV8HiUEOvI0CAAAECBAgQIECAAAECBAgQIECAAAECBAgQSCJAXMcyaKk+rJFelWrMdcyqURAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIELkFAgHgJ6Cd45EeTfEdDuw8leanhPrcQIECAAAECBAgQIECAAAECBAgQIECAAAECBAhcUQEB4vIn/u7N9qUtI7GFaYuSewgQIECAAAECBAgQIECAAAECBAgQIECAAAECV1hAgLj8yX8myWONw7gtyTuN97qNAAECBAgQIECAAAECBAgQIECAAAECBAgQIEDgCgoIEJc96Xdsqg9vbRzGPUleb7zXbQQIECBAgAABAgQIECBAgAABAgQIECBAgAABAldQYA0B4ieS3N4zd2vetvORJM+NWLdrmO8Rw3UrAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIDAWIE1BEpVUXfXFQ0Qh8LTXRZbmI59h7ifAAECBAgQIECAAAECBAgQIECAAAECBAgQIHDFBNYQIL6a5N6eeVvrtp0PJnlx5Hpdw3yPHLLbCRAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIExgisIVC6qgHi80keHjPZSdYw3yOH7HYCBAgQIECAAAECBAgQIECAAAECBAgQIECAAIExAmsIlF5K8kDPoNe4befdSa6PmejNvWuY7wOG7SUECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQKtAmsIlIYq8dYwxt35fDLJtdZJvnDfGi0OYPASAgQIECBAgAABAgQIECBAgAABAgQIECBAgACBLoE1BEpDYdoaxnhx/m5N8vaBS3ptFgcyeBkBAgQIECBAgAABAgQIECBAgAABAgQIECBAgMCaA8RnkjzWM8VrC83uS/LKgUt6jdu5HkjhZQQIECBAgAABAgQIECBAgAABAgQIECBAgAABAvsE1hCuXbUKxE8kuf3A5byG+T5w6F5GgAABAgQIECBAgAABAgQIECBAgAABAgQIECDQIrCGQOkqVSAeU31Y6+HOJBVAuggQIECAAAECBAgQIECAAAECBAgQIECAAAECBAjsFVhDgHiVKhD/SpLv7VjLP5fkuwbW+Rrm21uZAAECBAgQIECAAAECBAgQIECAAAECBAgQIEDghAJrCJSuSgXiHUne7FkLFSx+eGCtOAPxhG8mTRMgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIE1iCwhgDx1ST3dkzGO0kqNFvD1Vdp+ckk72/YnnQN872GuTQGAgQIECBAgAABAgQIECBAgAABAgQIECBAgMBsBdYQKH00yXf0CK9hjFV9eD3JrR3jrOrDjyR5t8fhc0m+dLYrUccIECBAgAABAgQIECBAgAABAgQIECBAgAABAgRmIbCGcO3jSf5wh+Znk/zuWUgf14nHkzzd08R2HvsCxHr5Gub7OEmvJkCAAAECBAgQIECAAAECBAgQIECAAAECBAgQ6BVYQ6DUt4XpP03yZStYA7UV6y0d43gqSW1vWpcAcQWTbQgECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAgcsUWHuAWLZLH+MjSZ7rWSR3Xjj7sC9AXNN5kJf5nvFsAgQIECBAgAABAgQIECBAgAABAgQIECBAgMCqBZYertXk9FXnrSFA7KuwfCFJBYzbSwXiqt+uBkeAAAECBAgQIECAAAECBAgQIECAAAECBAgQOL3AGgLETyS5vYdqyWO8O8n1nrFdrD6s2/oCxLeS3HH6JeUJBAgQIECAAAECBAgQIECAAAECBAgQIECAAAECSxZYcri2dR+qutsN2ZY0X3W24bWODr+c5MELv7svySs9g7OF6ZJmXl8JECBAgAABAgQIECBAgAABAgQIECBAgAABApckcBUCxKWO8dYkb/esi3uSvD4iQHwtSYWMLgIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQKdAksN1y4OaKgC8f4kdY7g0q6+isI3ktT2phevoQpEAeLSVoD+EiBAgAABAgQIECBAgAABAgQIECBAgAABAgQuQeAqBIhL3cK0Qs97O9bEQ0le2vldbWf6Ys8a2t3y9BKWm0cSIECAAAECBAgQIECAAAECBAgQIECAAAECBAjMXWDpAeJQ1V35L7ECsaoLr3csnk8luSNJnWl48eo7L7HueypJ3eMiQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAg0CkgQJzn4vjRJH++o2tPJHlmz+8EiPOcS70iQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECCxKQIA4v+m6Nck/TPIVHV27J8nrBwSIjyZ5fn7D1SMCBAgQIECAAAECBAgQIECAAAECBAgQIECAAIE5CQgQ5zQbX+hL31mGbySp7U33XUMViEvcynV+s6NHBAgQIECAAAECBAgQIECAAAECBAgQIECAAIGVCyw9QHw8ydMDc7S04OzVJPd2jOn7k/zMgQFiV+Xiype44REgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECIwRWHqAOFR1VxZLChBr+9K3eyawLwQcsrgtyTtjFod7CRAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIErp6AAHFec15nFD7c0aXXktzX092hAHHpcz2vmdIbAgQIECBAgAABAgQIECBAgAABAgQIECBAgMBKBZYeKvUFbtspeyjJSwuZv08kub2jr08lqZCw63okyXM9v1/6XC9kCnWTAAECBAgQIECAAAECBAgQIECAAAECBAgQILBsgaWHSn3nBW5n5tEkFTTO/arqwld6OnlnkgoYu66h1y99ruc+f/pHgAABAgQIECBAgAABAgQIECBAgAABAgQIEFiFwNJDpdeT3DUwE0OVe3OZyL5qyreS3DHQ0fr9mz33LH2u5zJP+kGAAAECBAgQIECAAAECBAgQIECAAAECBAgQWLXA0kOld5LcspIA8ZjtS7cEfW0sfa5X/UY0OAIECBAgQIAAAQIECBAgQIAAAQIECBAgQIDAXASWHiq92wC5hArEoe1H70lS1ZZDV9+Wrkuf66Gx+z0BAgQIECBAgAABAgQIECBAgAABAgQIECBAgMAEAksOle5Ocr3B4OUkDzbcd5m3PJPksY4OfCrJrY2d+5kkH+y4d+gMxcZHuI0AAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQGDNAlchQHwtSVX4zfnq23r02SSPN3b+A0l+ruPe+5NUhaKLAAECBAgQIECAAAECBAgQIECAAAECBAgQIECAQKfAkgPER5I81zC3cw8Qq7rw7Z5xPJTkpYZx1i19VZmPJnm+sR23ESBAgAABAgQIECBAgAABAgQIECBAgAABAgQIXFGBJQeItS3pi43zNudxPpnkWs84xva9zkq8a097S9jKtXE63UaAAAECBAgQIECAAAECBAgQIECAAAECBAgQIHAqgbHh1Kn6cUi7+84N/AdJ/uCexuY8zqoufKAD4IUkVWk55qptSu/d84IKFu8Z05B7CRAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIErp7AnIO1odnYF5R1BYh3JqlzBud21falbyapf/ddh5xbWNuUPtzR3lwd5jYv+kOAAAECBAgQIECAAAECBAgQIECAAAECBAgQuLICSw4Q62zDb9+ZuZ9N8t17ZnPMOYLnXAyPJ3m654GHBH59W7s+kaQqN10ECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIE9gosOUD8dJL37Izqzyb5sT0j/d4kH5nhGvhoku/o6NezSSpgHHvdneR6x4tsYzpW0/0ECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAgSsmsOQA8d09c1Vbfv61JLft/O7DST44s7m9Y7N9aVe3jqkWrKDwro6G6xzE+r2LAAECBAgQIECAAAECBAgQIECAAAECBAgQIECAwE0CSw0Qu8K32vLzLyd5/85Ia7vT+2Y2/31bjVZXD9m+dDvEJ5Nc6xjvC0kemZmF7hA4hUBV496SpP6t91tddRZq/VSIXp8L75ziwdokQIAAAQIECBAgQIAAAQIECBAgQIAAAQJLFlhqgFhh4Ct74Gs8zyd5eOd3c9y686UkD3Qsnr+f5JuPWFhD1Y1VoSk4OQLYS2ctcOumErkqkvuuChLrTNAK1b0fZj2lOkeAAAECBAgQIECAAAECBAgQIECAAAEC5xRYaoBYFXTP7UC9laSCs67KvmMq+k4xJ/u2YN0+Z4oqwb5tTA89X/EUDtokMKVAX/Vt13MqPKyw0da+U86EtggQIECAAAECBAgQIECAAAECBAgQIEBgsQJLDRD3VRluQ7Gu6sQKCF6dyUx19XHbvSn62rdFalVe1VmIqq5msiB0YxKBx5M8fWBL9V6oc0frs8VFgAABAgQIECBAgAABAgQIECBAgAABAgSutMBSA8QKAu/dmbmnklT1Udf2nRUO1HaFc7j2BaAX+zXVFqMVFN7eMeA5ecxhTvRh2QLHhIcXR/7RJH9mc07iskX0ngABAgQIECBAgAABAgQIECBAgAABAgQIHCiw1ABx3/afF6v29gWMryWpyr85XPv6t+3XlP3sC1Wm2CZ1Dpb6QKDOPLy++eOBKTRsaTqFojYIECBAgAABAgQIECBAgAABAgQIECBAYLECSwwQu7b/vHjG4b6ArkKBquy77OvuTdjR1Y+pg72+KsTaxtS5b5e9Ijz/WIGhit5D2q/Pi0eTvHTIi72GAAECBAgQIECAAAECBAgQIECAAAECBAgsWWCJAeIjSZ7bQd+t2us6/28OgVnf2YQ1rCnOP7zIU9u2PtaxSB8SkCz57avvSar68M3Nv6cA2W6NfIq2tUmAAAECBAgQIECAAAECBAgQIECAAAECBGYpsMQAsc45vLajuVu111WlOIcwoCqaHuhZDVOdf7h9RAUsb3c872NJvm2WK1OnCLQJDFUf/laSj2/OR60W6/Nj9/zUoSfVVsDPDt3k9wQIECBAgAABAgQIECBAgAABAgQIECBAYC0CSwwQa8vNu3Ym4IkkVWl38dq3dWf9b7XV6WVefVuKvpGktjid+vrFJN/Y0ejFrV+nfq72CJxSYOjsw09tzj3d3aa372zQrv6q1j3lTGqbAAECBAgQIECAAAECBAgQIECAAAECBGYlsMQA8d09gvu2Ju2q9LvMwKyrMnI7pH1B6BQL5k8l+amOhqbeMnWK/mqDQIvAUPVh3/vpA5vte7+k5UFJ6kzEeq84M7QRzG0ECBAgQIAAAQIECBAgQIAAAQIECBAgsFyBpQWI+wK4qjKqSqTda47bmO7bfvViv091RmNVNV7vWKYVtFZ1lYvAkgTuSPJKkvp339X1uXDx3qHzSHfbrfCw3qNzvOo9/g1JfnXTubeSVLWziwABAgQIECBAgAABAgQIECBAgAABAgQIjBZYWoC4b+vB1zbbFO4OvkLF+gL9lp1fVCVRnTN4Gdf/k+T3dTz4VNuXbh/3as/Zb0tbB5cxd545H4GhrUurp61bjlbwVpWMu9sid412DueoXuxb3x8l1Hu+fuqMWGHifNavnhAgQIAAAQIECBAgQIAAAQIECBAgQGD2AksLjvZ9Wd73hX7XNqaXsW3nULXTs0kqID3V1Xfu22Vu63qq8Wp3vQJDZxiODeOrirGCttsbyebyfun6fNs3jG2YuA0S6w8vhIqNE+42AgQIECBAgAABAgQIECBAgAABAgQIXDWBpQWI+6roHt1UEO2bu67Q7uUk9btzXs8keazngacONfvOX7wMj3Pae9Z6BIaC+B5oLXoAACAASURBVBrpIe+lqmqsz5eWSsS6r55xWVf19cWOyusxfWqt0hzTpnsJECBAgAABAgQIECBAgAABAgQIECBAYAUCawgQ+8KCrm1Mq/KmqojOedXWqbvbqV58/jnmosa9r8pqzme7nXOOPGveAvV+rnMPa9vRruuYUKwqEeu90Pc+3T637w8XTq1YfWwJOof64X0/JOT3BAgQIECAAAECBAgQIECAAAECBAgQuKIC5witpqTdF8LVeYb1v3ddc9jGtK/6r/rddY7jlHbVVl8V5D2b8GTqZ2qPwFQCQ1t2fipJhYB9nwdDfWmpcKw2PpPkWy5hG9ChSuah8e3+fmn/HzB2fO4nQIAAAQIECBAgQIAAAQIECBAgQIAAgQMElvbl8bt7xjg0hq5AoO/sxAMoe18y9KX/uaqZ+sKRJzYB49Rj1x6BKQSGgr1/nOSPTRSCD71ft+P5hSTfOsXgGtsY+kOExmZu3PbWJnAd+zr3EyBAgAABAgQIECBAgAABAgQIECBAgMDKBYbCtzkNvyqL3tzp0BsD2xnW7bXt4dt7BnLObUyHti89Z/XfvhC2eGxnOKfVri8XBeo9fH0g7Jr6DwJatwmd+rl9Mz9Ugbn72gpVf19Pg/5owPuMAAECBAgQIECAAAECBAgQIECAAAECBPYKLClA3Fd907r15/NJHt4jUOcgVpB4ymuocqolBJ2yf68mubejwaHtYKfsh7YItAoMVQS2fg60Pq/uq3MW670ydB5i/XFAVRBXuHfKa2z14f+S5N/YdKheW1cFsdvzI2ts9eMiQIAAAQIECBAgQIAAAQIECBAgQIAAAQI3CSw9QHw5SQV0Q9eTSa7tuen+M3yJPlQ19GySx4cGMOHv61lPd7R3rq1UJxyOpnoEaq4rLP7SJH8jSa21pV0twdmp/hBgKPy/aHnqKuKuz7B981nzXPcfcxbk0taJ/hIgQIAAAQIECBAgQIAAAQIECBAgQIDAhAJLChAfSfLczthbt+CrqpvaAnH3OkXl0u4zqsLx9p45O0eIefHx+7aC3f6+NZCdcAlq6gQCNcc/n+Sbdtr+lSR3LSxYGgrgTx16twZ3teVpvZdPFdoNfY5sp/qFJPVZ6SJAgAABAgQIECBAgAABAgQIECBAgAABAgcLLClA3PdF/pjzx349ye/ZkXpr4Fy1g2E3L+yr9qtbTv38rv53hRHnPBfyWFuv7xb4xSTf2PHrJc3xUHh3jj8AKMaf3mxTOrTmakvQh04QInad47rbn8v6PBly8XsCBAgQIECAAAECBAgQIECAAAECBAgQWJjAkgLEH0/ywzu+rRWI9bJ9Z/9VtVBtPXiqcxD/TpI/1LMmzr196bYrfWfKnbsicmFvmdl3dyi0rgF8W5KPzXwkFZq9cuHMvt3ufmrzu1O9dy8+r/pSlZBdZ4devPcU7+nWrVTH/EHFzKdf9wgQIECAAAECBAgQIECAAAECBAgQIEDgMgWWFCDWlozfuYM1JuzqClZOtQVi17ap2yGcMwDZXWN958qNCWUvc+169s0CFXTVVr21hWnf9Z8m+c9mDlhbgtZ2q13XucOysq2w8pYGtzGfSw3N5fkkDzfceKqzIBse7RYCBAgQIECAAAECBAgQIECAAAECBAgQWJPAkgLEfRWEY8KuriqeUwURQ2e3nWv7xa71+m7HLy67X2t6f517LPvOCd3Xhx9J8qFzd27E8z6Q5Od67r+sNdpaCVhBY4WIU1VHtpx/aPvSEQvMrQQIECBAgAABAgQIECBAgAABAgQIECDQL7D0AHFMpU9X1d3LSSoYmPoaqqD6L5L82akfOqK9vqqmJa2LEUNe/a19W9NeHPyY4P3caFXp978l+ZqeB49530/d/6FzGbfPqz8gqPMQj72qmvTNhkZO9YcQDY92CwECBAgQIECAAAECBAgQIECAAAECBAisTWBJQVGdV7i7feCYIKHri/hTVDMNbV9a6+iytxvsq1ar4KMCkLVdFU5tt8WseV/b1VVVujvOMe+bcxsNhaBzWJstFYHlNsX2yK2B5Zzn9NxryPMIECBAgAABAgQIECBAgAABAgQIECBA4EiBqxQgVnj0dofX1GFey5lll23f57GmaqYKSis0rFC3qlAvXv8oyf+U5H9IUpWoFVIv9eo713J3THMNm4bGMJdtOqufFbCf4zzEfVs3787nP0zy+0+wcKsyexu4V0V1vUdcBAgQIECAAAECBAgQIECAAAECBAgQIHAFBC47xBpDvK+6amwQ0lWhdS3JnxvTmZ57+4K57cteSFLB1mVfXdusVmhRtku9KiysOR27NW2Nu34qSNyGiVWpONVZdqf0HArfLj577PvmlP2+2HZfZd+vJvkjM5qL1vMma3xVRVih/NirdU6n3g65PsP+dpL37XS4QtOqqlxy0D52DtxPgAABAgQIECBAgAABAgQIECBAgACBKykgQPzCtH82yddO9MX40BaM9by5nEHXtz3iktbG9s1bwcdzBwSHQ2/+OWybOdTH1q0uq505zu1Q/6fYDnTIcOzvx4SIFUxXiFj/tl6PJ3m64eapA+GqzH1vx3OX/scFDZxuIUCAAAECBAgQIECAAAECBAgQIECAAIE5Bgn7ZqXr/MKxX5z/3ST3dEz7FCFRBVjXk1R/+67bJgorj13BXa7VbjlVheJSrqo2rPCw5mDqqxy61s3Uzzq0vZatLrdtj33fHNqn1tf1rcNqYy4Vu/vG0/IHAxdfV0Hps43v/6r4e2AAceozXFtC0TmGua1rzX0ECBAgQIAAAQIECBAgQIAAAQIECBAg0CCwlACxtqSsYG73GhuE/ESSH+xwqS/1q+LnmOsyvvA/pr/12tqOcN9Zbks5B7ECw/86yXcdCzHw+rm/V356s71kC8NcAuxtX7u20q3ff2oTyM9528w/neQnW+A399RWrRVID42pJRSe4nPrYtf/apLvHhjL1M8cQedWAgQIECBAgAABAgQIECBAgAABAgQIEDiHwNxDka1B11lgYwPEvuqa+jL/zoYv9bvmpeXsw3rt2D6feh10VVC9fIKtQKccS3k/vDlf7hRVhxf7+lZDVemUYzukrb+U5AcaXzin931Vjr7Y0++5vV+6ujq0Bevu6ypErLH1na/ZFe5fbGtqn75zKLfPnbrqsXHZuo0AAQIECBAgQIAAAQIECBAgQIAAAQIEziUwpyChb8xTBYi1VWJVO+2ruKvnH/Nl/P+R5A8OTNwcv3jvC1XnVqm25a0+XztjqDeXMyv7lteYrTTnsj3t0NalS6mC3c7L0Hh2528opH+34f8I6o8e+kLIhia+6JaWZ76RpKrCXQQIECBAgAABAgQIECBAgAABAgQIECCwUoGrFiDWNPYFLYduzVdbnz7dsEbmGsh1VR0dE6g2cIy+pSoNq1qtAuUprwpyKliunwpGymN7jmVtI1k/c7/GVMDNZR32vRcrpKp5Htrmc27zUuF2javrjxQu9ve3k3x9RwDYWtE85Wd46zOXcCbo3NaF/hAgQIAAAQIECBAgQIAAAQIECBAgQGBRAlN++XzKgU9VgVh97AtaKqyocGXMVV+61/mM28Cp67VD1UZjnjn1vV1nrf1HjcHo1P3Z114Fe68kmXK70r+R5N/bhFS1xv5ikvdtHv5LSf7MQsLD6vKY6rc5BMNDgecc+njMum45D7XafyFJhY67V8t8Tl0J2PLMbT+X8v8dx8yh1xIgQIAAAQIECBAgQIAAAQIECBAgQODKCizlS+ApA8ShKpuxWwIOBSG1uP5Zkn95xtVUXduYzqXSqOa/Kg+PDQ8rcKmwdFttWP/W9cNJfrzjU+BHknxoIZ8QLWfm1VAeSlIB12VefX1dwpaxLXYtlcldf7TQ9Zl38bn/fZJ/u6UjjfeMCRDnUsXaODS3ESBAgAABAgQIECBAgAABAgQIECBAgMAYgaUHiIcGIV1bdpZdV0XQPteqiqvqw6Hrw0k+OHTTJf++6+yzy14j35Lk40l+14E+FRrWlpIVHO47K65Ckzq/8ks62v9ckn914nPmDhzK4Mu6Kkl3X3jZZwsObV26pvP1WoLAfe+xvrNJt/M5dbjd0tftsx9N8vzginQDAQIECBAgQIAAAQIECBAgQIAAAQIECCxS4LLDoVa0ri+2Dw0Q64vvh3sefs+mSq2vf61VcZ/abC8597Pc5ngOYoV7FdCOrTz8/5L87U3l4FCl3V9K8gMDC/Gnk/zJ1sV6ifc9uKnUHOrCzyb5nqGbTvT7oaq8pXwmjeHpCue3bez7vGmpbJ56m9cxAeKYP7QYY+VeAgQIECBAgAABAgQIECBAgAABAgQIEJiBwFK+rO/6YvvQSqqWL8r//ST/zZ45qjDrWpIKQlqupXzR3hXsXGb/a4vRu1qQL9xTZ01W+LLdnnTo5S3PeC1JrZkprj+a5L9M8nVJqrrxF5L8BxNVOLZWxL6Z5A9MMZiRbVQgXFvRdlUYHvp+HtmNs98+VBm67w8hhl5zyHmtQwNvqXq82MZatpodcvF7AgQIECBAgAABAgQIECBAgAABAgQIXDmBqxogVghYFXe3DMx4Va/Vl/vb608l+QtJvrxxpSyl+rCG0xWqThmeNbJ9vuLwuSRVUdd61ValFYC0Bofbdn81yVcPPGSqs+Y+kuRP7HnWbyT5wGab1dbxdt3Xtz3v9jW/ecSWsMf0ry8UW8I2v4eOfSgM3BcgDs1jfbaMrcwd6n/LH1bstlF/ePDsUMN+T4AAAQIECBAgQIAAAQIECBAgQIAAAQLLElhKgFiVS1U1tXsdU7HUdw7bxedUpU8FANWHsWez1RfrrZWKl71yKox4u6MTLVu6Ttn/1rmpZ1aQUsaHnsc2tL1kPWOKKszvS/KXe5A+k+T3Jzl2q9uWrS+rG3dOVPXYOu9D1W3nXmOt/T72vnpf1WdXX9i3uxVp33tx259TBPuta2fXpP7Qos5EPHbtHmvt9QQIECBAgAABAgQIECBAgAABAgQIECAwkcBSAsSuL9SPCRC7QsmJaFMVcWMDx6mefWg7XWdDVjhwaEA3ti9DZ+RdbO/Xk/xrRwRhrWtgiq0afy3JVw1gTOHcuo3pOcPtoRBtCt+x6+xc9w+dt1r92A1PWyoBj/ns6xp7BYEPHAhT4WEFkLWFcFVPuggQIECAAAECBAgQIECAAAECBAgQIEBgwQJLCRCLeF+l2LEhyJiwasw0V1VcBTlL+yK9qwKpQoEx24mOsbp4b2v4Va/5dJLbj6x6aglq6lnHhjVlV2f/DV3Hrudt+7VF6ZcNPOwUZ+h1PbIvRDvFVpxDzuf4fYWm/12Sb2142O7ncMvn0r5tTxse1XvLMQHituFtxXa1VX9EMXZL4WPH4PUECBAgQIAAAQIECBAgQIAAAQIECBAgMIHA0gPEKbaWrC+475rActvEoWfxTdiFg5vqC/DOsVZa56JCpwr/jg0nWgPEYyvkWqrQatKmCmr/SeMZh8cGoy0Lbch4d/vOljbnfk9t11rb8A6dsVrj2PcZ1rKF7ym2fG3ZzvcQ+9oCut6r9SNUPETQawgQIECAAAECBAgQIECAAAECBAgQIHBmgXOEQlMNad+X21OdA1btfPsEHa0vx6vabGmVhxeHXhVE+4KPUwQWF587dEbe9t6pwsNqr/WZx1R7VSXa9c0ZmkNLbIpAr+X8vG0/ap3WWYinuqovr/Rs5TvV+/fY/tdWtg9v5ujie7feCzWG7b/1nLr3a5K8N8lv76mArd/XT+u1731Vgdu9Aw1M/dk9FPS2jqflvtp6+G8m+YXN+bK1DlwECBAgQIAAAQIECBAgQIAAAQIECBAgMCOBqb+EPuXQThkgVkhQW+4NfWnfNb4KtWr7z6ocWvrVVS03RbjVZ9MSmtTrpzgncNuPri1bd/t5zNhbty+damxjg6BjwtGhtT7ke9uRW9AOPb/l9xUiP70JClvun/Keri1rhypxT3G+6n+S5D+fcnAj29pWKFaAW58Fby38DzFGDt/tBAgQIECAAAECBAgQIECAAAECBAgQmJfAkgLEfZVxU1cwVdhTAVrL1oM1k7+V5KNJ6syyJVcdXlyVXeevTbW95r53QGvodUyQd8xzjwkth0K0i/2aosrzx5L8xyM+Zk5VhTg0p1PP5Ygh37i1KgWrOrT+gODcVwVkXZWKQ2H6Kd6LQ8/8ZJKvGPHZOJVnBYv12V8/2y2Lq691CRmnUtYOAQIECBAgQIAAAQIECBAgQIAAAQIEdgQEiDcviQoTKkSr8wAf2LNiqtqwvsCuisUKG9d2Vajx5p5B1Rf4VTF2iusnkvzgQMOnqLpqrQ48tEpvzPalNfwp3o8/l+QDIydp6nMIa9wvbs6p3NeVCn7q/VVr6jKvMeHu1P3sMx86i/AU4WvX1sXbcdd79Cc3n3u3T40xQXsVhNfPxW1nK3Csz7PdP+6o/15rtOsM1fqMr2vb3gTd0wQBAgQIECBAgAABAgQIECBAgAABAgSWJTBFYHGuEe/b1m/qCsTdsdSXzBV0bK+LVTDnGvdlPKcrTJg6aNqOraqbvnZgoKd49lCV3LZLL2zOSxw7F63tV7tTBaS/kuTrRnZ06vdRbeX7WE8fTjGXI4f8+dvrjwD2/ZHAIW21vqaqlv+tzRmAXa8ZChCfmHi75JYgfVuFu/0Di2utA75C91XgWH9s0BVMXiEKQyVAgAABAgQIECBAgAABAgQIECBAYOkCSwoQz7GF6dLnc6r+dwVAUwcX1d+WkO03NtsnTjW+bTsVhrzd0OgvJ3lfw327t4ypcJsqxBsKn7qGcedE2/DWfD7Xsz3noWHsAfyDLxkzP4ONNd7QEp4OzeEUW91e7O5Q4Fv37q6P+sOKqtR+uHHcV+W2U2wve1XsjJMAAQIECBAgQIAAAQIECBAgQIAAgRkJLClA3HdG11Shy4ymZBZdOec5iF3PugjxF5L8hyeS+b+T/EsDbf+TJO8dueVmhZO1FWzr+XpTrOWWMLZrqFMEH0Njrq0ha0vJy966dGtwjNfY5VjzW2t9qDqtpU8tIWRr/4bmrNqpeetaxzWfj2x+5ri1aavDVPedcqvnqfqoHQIECBAgQIAAAQIECBAgQIAAAQIECAwKLClArO3hdr+gniJ0GUS6gjd0nYNYFFOvmaGz1+qZUwYmu9PZembg2OrLClWqEq/1+nSSW1pv7rivJYzte8SxVYhDFX2nnMdD6Y41G3puhW/lUlV+LVffe2/7+infgy3blz67CT+H+l8hYwWgVZ14cfvne4deuKLfT7UV8YpIDIUAAQIECBAgQIAAAQIECBAgQIAAgSUKTPlF9KnHrwLx1MJf3P6+wLbumHL7xJaw5K1NIHGqqrU/neQnG2ircqwCsNZ+HHK+3rEB2yHPvDj0Y7YXHdoG85i2G6bnqFsq9KogcVtltz33dBvoVghY/7nCoZr/r0jyuQs/tY7r/VJB2faeWi/PN1Qc7na8JdCb8nO7631+sV91rl+trWOvcvrmJF+/sS73tYWL27Mij7XyegIECBAgQIAAAQIECBAgQIAAAQIECFyqwJRfRJ96IPsCxCm2XTx1v5faflcYNbYSr2/8LdVfrdVPxzgPnTm3bfupTTXZ0LMqiPpkkt89dOPO748J2eqZ13vOHmzpyj9K8nUtN+7cM3SWpPdpO+pQgNi3nWj7U75w59C81T1TPq+rfxXYVpi4rWCsf+8aMZgKc387yW9uXnNsJW/fo+sPGipEruC1+llBcf3nuur/o4a2qB0xLLcSIECAAAECBAgQIECAAAECBAgQIEDg8gSWFCDWF7O7XyrbLu50a6frLLYpt41tqZg7R0VPSxXWVrolRGw5x65r5g4d79gtU7uePzYgrrH+9STv6WiwAqi6R7DS9l4d2ga21mptNTvF1fL+G7sepujXxTaqarGui2dnbgO7lmrgCifrvm0727a31ab1b/3UPdt/t+1v763fWb9Tz6z2CBAgQIAAAQIECBAgQIAAAQIECBCYtcCSAsR9IU9Vg+x+MTxr8AV1rqs6acoAY19V6S7ROdbox5K8f8TcVPBSQV9XgNFSWdn3uAorakvQqkhsuSokeeXCFpxdr6lKwAcGGhwTENcaqedut/zc1/RtI7Z9bRnr2u8ZChDHzE+fVUv1Yb3+2HMx1z5fxkeAAAECBAgQIECAAAECBAgQIECAAIFVCpwjnJkK7jf2bAlZ/1udR+Y6jcC+qs960rFn9W17WwFc33aD/2eS955maDdarQC6tv7cViS1Pu5Xk3zvZtvC3dfU2XcPtzbUc19tzfjxJB9KUsHRvsCy+v/iQIi3fUSdZVdh37WBvrXMb3m9OeB2zJasE/AtsomhAHGqP5poWaPn2D54kZOk0wQIECBAgAABAgQIECBAgAABAgQIEFi7wJICxK5z6pY0hqWtp64w49BtNnfHPxQg1v0/mOS/OiHcDyf58SPaL6MKWi6Ge13B6xGP+fxLq2Jz+1Nbg/7Jjc+XNjRc91d4WBWkQ1u2DlW5VXhYoWVtTdp1fTjJBxv65ZYvFhiqXp3iTMIKnSv8HbpUjw4J+T0BAgQIECBAgAABAgQIECBAgAABAgRWKrCk8E2AeP5FWIFTVeftXkMBU2tPW7Yw/fUkP7rZzrPlzLPWZ2/vq0rCrx77op37q18VJFbF3fck+akj2zvFyy+e3ThU5VbP76pCrPDwuSQPDnTS1peHzeJQgFitHvu53TL/U73HD1PwKgIECBAgQIAAAQIECBAgQIAAAQIECBC4VIFjv4g+V+f7KmYEFaedhX3ValOdg9gSZFwc3bZ6rir8KrSr/17n+h0aLLaENafVPU/rVbVW76ExVZLluhsStoSH9axHktQ5ka7xAt+Z5OcHXnZMZWBr9eFdSf7e+O57BQECBAgQIECAAAECBAgQIECAAAECBAisQWApAWIFF293gC9lDEtdL11npbWckzc05toC85Whmxp/X6FiBZJ1Rlz956Gr5Qy/oTYO/f2nk3zloS8e+bquQK8lvN2d45aKUefmjZygndtb3hPHvPcq2H1goIu1PvvOJj1uhF5NgAABAgQIECBAgAABAgQIECBAgAABArMXWEr41vel+jFfps9+gmbQwS77hyaqMvtfk3zzxOOsysRnNtWJ9Z93r6qsq204K0S8jOsnknxjknvP8PAnNha7j6qxl01fUFRB7D2bF7ac61hbuFZV56EVoWfgmP0jWioEa05aQvLdwVZlaK37oatrzQy9zu8JECBAgAABAgQIECBAgAABAgQIECBAYCUCAsSVTOQJh9EVaOzb4vKQblT7v5Tkyw55ccNrqmquqq7qOd+e5MuTfFPD67a3fCbJe0bc33LrdgvKqgKswO1U1V4Xzz3c16+WKsQKiquPQ2FnVX7WmZnCw5YV0H9P13mv21cd8kcTtf7rPNOW0Hwp/79wvLQWCBAgQIAAAQIECBAgQIAAAQIECBAgQGCvwFK+KFaBeLkLuKr5HtvThanWT2tl1LkVajvOGntt4zoUoLX2bXeLzwrdqiKyDG5vbaThvpYK0QqVqpKtL8D8f5P83oHn7TtjsaGLbtkjUOuhgr6+69HNmmwFrDZrq+CW8HAodG59pvsIECBAgAABAgQIECBAgAABAgQIECBAYMECUwVApyYQIJ5auL/9fZVq/zTJV09YcVYh2ouXO8wvevpuRV31r8LEY0K+15LUWt53VbhTQc8PJfk3k3zVgRb/OMn3JKnKy5arKxxueW3dU+Fh9XvfVrGtbbjvnwv0nfe6vWtMBWK19783ridBsJVIgAABAgQIECBAgAABAgQIECBAgAABAp8XWEqAWFsoPt0xZ2OrcUz9eIGqVPvlJL9z56VTVytVFV4FWqfa0nPMyLsq+A4N3HYrD4f6UsFPeVRwWQFdi8kh89FyFmJXXytwqv61hpVDY/b7LwTMVS3Yd7UGiDW3/3OSr2+EbalabWzKbQQIECBAgAABAgQIECBAgAABAgQIECCwZIE1BIhPbEKnJc/DEvr+8SR/eKejfRV1h46pwsqqeHz40AYmeN3QmqpAr0Ltlj5+NskHN+cwHtq1CoIqWKrn1r9ltK34q3/rjMf6OfRqOQtxt+0KDyvgPOa5h/Z3za+bKkCsNVNBZK2ZlmtswN3SpnsIECBAgAABAgQIECBAgAABAgQIECBAYKECSwkQ+wKOQ6quFjpdl9rtfZV37yS550TbV27Dsqpwm+r8wRbAMUHK9vzC+veBTeMVrJXL30/yY0n+ZstDZ3BPBZGt27MKD083YVMEiBVuX2s887BGUn8IUO+zWrcuAgQIECBAgAABAgQIECBAgAABAgQIECCwmC1MBYiXv1ir6u3NPd0YqtabsucVrmzPCvzWJN+e5HdN9IA687CCl6taUTfmDEpbXU606PY0U2H09YHmvz/Jz+zcU6+r0HD7HmntYa37em+7CBAgQIAAAQIECBAgQIAAAQIECBAgQIDADQEViBbDGIF9VWqvb6oQx7Qz5b0VflT4dWilYlXTPb/ZNvWqV2B9MsnXDkyOM0enXL03t9USIP52ku/eVAzWNrL/bpLfc0C3/lmSP+4MywPkvIQAAQIECBAgQIAAAQIECBAgQIAAAQIrFxAgrnyCJx5eBW37zv2rbUwrSLzsa7ulaFUS3jLQmdq2se6bQ78v262e/2rjVrFzmes5mJ2qD++equGddr83yUfO9CyPIUCAAAECBAgQIECAAAECBAgQIECAAIEFCQgQFzRZM+hq1/lsL28qAGfQxRtd2J6hWP9WZeG2urC2KBUafvFM9W0RvDunKhBPv8r3nTc69VPHnPU59bO1R4AAAQIECBAgQIAAAQIECBAgQIAAAQIzFxAgznyCZti9fduYVjdVps1wshq6NObsw2qugtjbGtp1y+ECY+dk7JPmGPiPHYP7CRAgQIAAAQIECBAgQIAAAQIECBAgQOCEAgLEE+KutOmu6igVM1G/VwAAEixJREFUTcub8K6K0qGR3JmkgmTX6QQ+luT9J2j+hc3WvVf9vM8T0GqSAAECBAgQIECAAAECBAgQIECAAAEC6xEQIK5nLs81ktoS9Pqeh1UgUVWIgqVzzcRxz7k1yStJaj7HXs8l+YGxL3L/KIGan19J8p5Rr+q/+Ykk9QcALgIECBAgQIAAAQIECBAgQIAAAQIECBAg0CsgQLRADhGosPCWPS+8P8mrhzToNWcXqLMgHzjiqUv57DhiiJf+0qoQre1Gv/LInlR1cJ1zqerwSEgvJ0CAAAECBAgQIECAAAECBAgQIECAwFURWEoIUF9+X+uYlKc2X45flTmbwzgrJLx3T0cEiHOYneE+tJ6x9z8m+WMdzZnrYecp7rgjSYW9d41s7I3N52K91kWAAAECBAgQIECAAAECBAgQIECAAAECBEYJCBBHcbl5I/B4kqd3ND6VpMIOVU7zXiY1R7UFbW2R2Xdtz7R8t+Om2qq2zkJ0nUeg/oii3nf7Kn+rBxUY1pw8vwkcz9MrTyFAgAABAgQIECBAgAABAgQIECBAgACBVQoIEFc5rWcZVJ2l9sgm0FDtdBbySR5SAdPDAy1dDIP77q8A0ZmXk0xLcyMVAFcFaQXAW/v619bBzYRuJECAAAECBAgQIECAAAECBAgQIECAAIEhAQHikJDfE1iPQAW+zzUM56ELVWwVVL3d8ZoXNiFyQ5NuIUCAAAECBAgQIECAAAECBAgQIECAAAECBJYiIEBcykzpJ4HjBWp72a4tMLetv7ypcLv4tKpwu33P421jevycaIEAAQIECBAgQIAAAQIECBAgQIAAAQIECMxOQIA4uynRIQKTC1QVYVUe1taXfVdtXXr3nm1J+yoX77d95uTzpUECBAgQIECAAAECBAgQIECAAAECBAgQIHCpAksJEPvOYfvJJD90qYoeTmDeAnVe5WMNXXwiSd27e9W5e292vN42pg2wbiFAgAABAgQIECBAgAABAgQIECBAgAABAksSWEqA+EtJvqED9peTvG9J6PpK4IwCVT34U0n+xYFnDgWBXduYvp7knjOOx6MIECBAgAABAgQIECBAgAABAgQIECBAgACBEwssJUB8t8fhM0m+8sROmiewRIEnk1xr6HhtXVpVhnVGYtfVV8VoG9MGZLcQIECAAAECBAgQIECAAAECBAgQIECAAIGlCKwhQCzrpYxjKetCP5cv0LptaYWHdTbiqwND7jsH8dkkjy+fzAgIECBAgAABAgQIECBAgAABAgQIECBAgACBpQRvdye5PjBdtYVibaXoIkAgaa08LKvW8O/WJG934Fbl4p0DFYzmhQABAgQIECBAgAABAgQIECBAgAABAgQIEFiIwBIq9+5L8sqApy0UF7LgdPPkAi3vl20n6lzDCt/7ti692OFfS/JVHSN4KMlLJx+dBxAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQInF1hCgNhSTfVEktqy0UXgKgtUlWBV69Z5hkPX/5XkfSPCw2rvI0n+REfDrZWMQ/3yewIECBAgQIAAAQIECBAgQIAAAQIECBAgQOCSBdYSID6VL2zb6CJwlQVaqw/r3MO6d+y2v33nIFYVY1UzVlWjiwABAgQIECBAgAABAgQIECBAgAABAgQIEFiwwBICxNoW8YEB45eTPLjgedB1AlMIvJrk3oaGDt3ytyocKyC8peMZtjFtwHcLAQIECBAgQIAAAQIECBAgQIAAAQIECBCYu4AAce4zpH8E2gRq29I3G259NMnzDfd13VJbBT/W8cuqaKwqRBcBAgQIECBAgAABAgQIECBAgAABAgQIECCwYIElBIgtVVWvbbZkXPBU6DqBowQeT/L0QAsvJKltSI+5hp5zp21Mj+H1WgIECBAgQIAAAQIECBAgQIAAAQIECBAgcPkCAsTLnwM9IDCFwN9J8ocGGpoq3KttTG/veJbzSKeYTW0QIECAAAECBAgQIECAAAECBAgQIECAAIFLFBAgXiK+RxOYUOCdnrMJ6zFvJ/m9Ez2v71zSChcrqHQRIECAAAECBAgQIECAAAECBAgQIECAAAECCxUQIC504nSbwI7AuwMifyXJ902kdl+SV3raui1JBZouAgQIECBAgAABAgQIECBAgAABAgQIECBAYIECAsQFTpouE9gRGAr06vaptxbt28b02SR1VqKLAAECBAgQIECAAAECBAgQIECAAAECBAgQWKDAWgLEF5I8skB/XSYwhcCtmy1K+9p6IskzUzxs08bzSR7uaO+NJHdP+CxNESBAgAABAgQIECBAgAABAgQIECBAgAABAmcUWEuA+FqSqsJyEbiKAi0B4tRVgX1Vj59L8lW2Mb2KS9GYCRAgQIAAAQIECBAgQIAAAQIECBAgQGANAksIEF9K8sAA9stJHlzDhBgDgQMF6szBW3pee4qQ/bNJvrzjmVNXPB7I4mUECBAgQIAAAQIECBAgQIAAAQIECBAgQIDAWIElBIi1NelzAwN7NEltqegicFUFXk1yb8/g68zCOyfG6Qv3bSs8MbbmCBAgQIAAAQIECBAgQIAAAQIECBAgQIDAuQSWECCWxW8m+bIOlN/qqYI6l6PnELhsgSeTXBvoxD1JXp+wo48nebqnvQosK7h0ESBAgAABAgQIECBAgAABAgQIECBAgAABAgsSWEqA+LEk7+9w/VtJ/uiCzHWVwCkE+s4k3D7v/iRVqTjVdUeSN3saUxk8lbR2CBAgQIAAAQIECBAgQIAAAQIECBAgQIDAGQWWEiDW+YYvdrg8lKS2UnQRuMoCdye5PgBQFYp/bmKkvq1T61zG2yZ+nuYIECBAgAABAgQIECBAgAABAgQIECBAgACBEwssJUDsq3SyTeKJF4nmFyPw2YHtfE9xLuHQNqZTVz0uZjJ0lAABAgQIECBAgAABAgQIECBAgAABAgQILFVgKQFi+b7bgbykMSx1nej3MgQ+k+Qrerr6WpLa6nTK69Ykb/c0+FSSOp/RRYAAAQIECBAgQIAAAQIECBAgQIAAAQIECCxEYEnhmwBxIYtKNy9NoG870erUJ5JUxe7UV20h/EBHoy8nqS2IXQQIECBAgAABAgQIECBAgAABAgQIECBAgMBCBJYUIL6e5K4d1/rf7lmItW4SOLXA80keHnjIKd7zjyR5ruO5p6h6PLWj9gkQIECAAAECBAgQIECAAAECBAgQIECAwJUWOEWYcCpQFYinktXuWgRqq9BrA4OpwL2C9ymvH03y5zsa/AdJvn7Kh2mLAAECBAgQIECAAAECBAgQIECAAAECBAgQOK3AkgLE2n7x9h2Ot5LccVoirRNYjEBtFfriQG+fSPLMxCP6iSQ/2NHmLyd538TP0xwBAgQIECBAgAABAgQIECBAgAABAgQIECBwQoElBYiPJ3l6x+IUYcgJuTVN4KQCdye5PvCEOq/woYl70Xf24htJql8uAgQIECBAgAABAgQIECBAgAABAgQIECBAYCECSwoQi/S+zU/95wot6sdFgMA/F3gnyS0DILclqfumuvZVB2/bfjZJhf8uAgQIECBAgAABAgQIECBAgAABAgQIECBAYCECSwsQF8KqmwQuTaAqDB8YePoLSR6ZqIe3Jnm7p62qdqw+uQgQIECAAAECBAgQIECAAAECBAgQIECAAIGFCAgQFzJRukmgUaCqdF8ZuLeqD+9JUpWDx15Dz7tfpfCxxF5PgAABAgQIECBAgAABAgQIECBAgAABAgTOKyBAPK+3pxE4tUBVBL6e5PaBB308yR+ZoDNPJrnW047PmAmQNUGAAAECBAgQIECAAAECBAgQIECAAAECBM4p4Mv9c2p7FoHzCNSZg083POpHknyo4b6+W/rOP3wjyd1Htu/lBAgQIECAAAECBAgQIECAAAECBAgQIECAwJkFBIhnBvc4AmcQaK1C/PUk37apWDykW0NB5ZRnLR7SP68hQIAAAQIECBAgQIAAAQIECBAgQIAAAQIEDhAQIB6A5iUEFiAwdDbhdgh1HuIzSSrsG3MmYrX/YpIKK7uuR5M8vwArXSRAgAABAgQIECBAgAABAgQIECBAgAABAgQuCAgQLQcC6xWosxDvGjG8V5O8tAkTK1jsuoYqD7ev8/kyAt+tBAgQIECAAAECBAgQIECAAAECBAgQIEBgLgK+4J/LTOgHgekF6vzBv5vkkPf5x5N8JMnLFyoTq71rSR5s6KrzDxuQ3EKAAAECBAgQIECAAAECBAgQIECAAAECBOYocEiwMMdx6BMBAvsF/mKSH5oAp7Y3vWNEO/cccbbiiMe4lQABAgQIECBAgAABAgQIECBAgAABAgQIEJhaQIA4taj2CMxLoM4orK1Jx2xleuwIXktSZyS6CBAgQIAAAQIECBAgQIAAAQIECBAgQIAAgQUKCBAXOGm6TGCkQIWIVUF4y8jXHXL7W5stTuv8RRcBAgQIECBAgAABAgQIECBAgAABAgQIECCwQAEB4gInTZcJHCBQ5xc+f4ZKxPs3FY8HdNFLCBAgQIAAAQIECBAgQIAAAQIECBAgQIAAgTkICBDnMAv6QOA8AlWJ+GSSx07wuE8m+X7h4QlkNUmAAAECBAgQIECAAAECBAgQIECAAAECBM4sIEA8M7jHEZiBwB1JHklybaK+vLE58/CdidrTDAECBAgQIECAAAECBAgQIECAAAECBAgQIHCJAgLES8T3aAIzEKitTasq8V9P8t4k/8KIPtV5h89sfka8zK0ECBAgQIAAAQIECBAgQIAAAQIECBAgQIDAnAUEiHOeHX0jcF6Bqkx8MMm2QvGWjse/luSlzZmKqg7PO0eeRoAAAQIECBAgQIAAAQIECBAgQIAAAQIETi4gQDw5sQcQWKxABYnbn09sRlH/bv/zYgem4wQIECBAgAABAgQIECBAgAABAgQIECBAgEC3gADR6iBAgAABAgQIECBAgAABAgQIECBAgAABAgQIECBA4IaAANFiIECAAAECBAgQIECAAAECBAgQIECAAAECBAgQIEBAgGgNECBAgAABAgQIECBAgAABAgQIECBAgAABAgQIECBws4AKRKuCAAECBAgQIECAAAECBAgQIECAAAECBAgQIECAAIEbAgJEi4EAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQGiNUCAAAECBAgQIECAAAECBAgQIECAAAECBAgQIECAwM0CKhCtCgIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIEbggIEC0GAgQIECBAgAABAgQIECBAgAABAgQIECBAgAABAgQEiNYAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQI3C6hAtCoIECBAgAABAgQIECBAgAABAgQIECBAgAABAgQIELghIEC0GAgQIECAAAECBAgQIECAAAECBAgQIECAAAECBAgQECBaAwQIECBAgAABAgQIECBAgAABAgQIECBAgAABAgQI3CygAtGqIECAAAECBAgQIECAAAECBAgQIECAAAECBAgQIEDghoAA0WIgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQECAaA0QIECAAAECBAgQIECAAAECBAgQIECAAAECBAgQIHCzgApEq4IAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIAAgRsCAkSLgQABAgQIECBAgAABAgQIECBAgAABAgQIECBAgAABAaI1QIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQIDAzQIqEK0KAgQIECBAgAABAgQIECBAgAABAgQIECBAgAABAgRuCAgQLQYCBAgQIECAAAECBAgQIECAAAECBAgQIECAAAECBASI1gABAgQIECBAgAABAgQIECBAgAABAgQIECBAgAABAjcLqEC0KggQIECAAAECBAgQIECAAAECBAgQIECAAAECBAgQuCEgQLQYCBAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAQIFoDBAgQIECAAAECBAgQIECAAAECBAgQIECAAAECBAjcLKAC0aogQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgQOCGgADRYiBAgAABAgQIECBAgAABAgQIECBAgAABAgQIECBAQIBoDRAgQIAAAQIECBAgQIAAAQIECBAgQIAAAQIECBAgcLOACkSrggABAgQIECBAgAABAgQIECBAgAABAgQIECBAgACBGwL/PyNAbKVdX3iWAAAAAElFTkSuQmCC"
                }
            };
        }
    }
}
