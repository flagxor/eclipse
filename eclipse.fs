#! /usr/bin/env gforth

s" grf.fs" included
s" eph.fs" included

1024 constant iwidth
1024 constant iheight
iwidth iheight * 4 * constant image-size

variable fh
s" data/map.bgra" r/o open-file throw fh !
create map-image image-size allot
map-image image-size fh @ read-file throw drop
fh @ close-file throw

: draw
   0 0 pixel
   height 0 do
     width 0 do
       i iwidth width */ j iheight height */
       iwidth * + 4 * map-image + over 4 cmove
       4 +
     loop
   loop
   flip
;

: handle-events
   wait
   event expose-event = if draw then
   event press-event = if
     last-key 13 = if bye then
   then
;

: main
   2457980.5e
   30 0 do
     fdup in-time sun earth v- dayrot fdrop fatan2 >deg f. cr
     .03e f+
   loop
   fdrop
   1024 1024 window
   begin handle-events again
;
main
