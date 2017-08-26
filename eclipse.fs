#! /usr/bin/env gforth

require grf.fs
require eph.fs
require ray.fs

1024 constant iwidth
1024 constant iheight
iwidth iheight * 4 * constant image-size

variable fh
s" data/map.bgra" r/o open-file throw fh !
create map-image image-size allot
map-image image-size fh @ read-file throw drop
fh @ close-file throw

: draw-map
   0 0 pixel
   height 0 do
     width 0 do
       i iwidth width */ j iheight height */
       iwidth * + 4 * map-image + over 4 cmove
       4 +
     loop
   loop
   drop
;

variable circle-color
variable circle-radius
variable circle-sx
variable circle-sy
fvariable fwidth  fvariable fheight
: draw-circle ( x y r c -- )
   circle-color !
   width s>f f* f>s circle-radius !
   height s>f f* f>s circle-radius @ - circle-sy !
   width s>f f* f>s circle-radius @ - circle-sx !
   circle-radius @ 2* 0 ?do
     circle-sy @ i + height mod
     circle-radius @ 2* 0 ?do
       circle-sx @ i + width mod
       over pixel circle-color @ swap l!
     loop
     drop
   loop
;

hex
ffffee00 constant orange
ff777777 constant gray
ff000000 constant dark
decimal

: draw
   eph-time f@ fdup f. julian.
   draw-map
   sun earth v- eph-time f@ longlat .03e orange draw-circle
   moon eph-time f@ longlat .015e gray draw-circle

   earth moon v+ sun ray
   earth earth-radius sphere
   intersect if
     hit earth v- eph-time f@ longlat .015e dark draw-circle
   then

   cr
   flip
;

: seek ( f -- )
  begin
    fdup +time
    earth moon v+ sun ray
    earth earth-radius sphere
  intersect 0= until
  begin
    fdup +time
    earth moon v+ sun ray
    earth earth-radius sphere
  intersect until
     
(
  begin
    sun earth v- vunit
    moon vunit vdot
    0.9998e f> if
      fdrop exit
    then
  again
)
;

: handle-events
   wait
   event expose-event = if draw then
   event press-event = if
     last-key 13 = if bye then
     last-key [char] 1 = if -.001e +time draw then
     last-key [char] 2 = if .001e +time draw then
     last-key [char] q = if -1e 48e f/ +time draw then
     last-key [char] w = if 1e 48e f/ +time draw then
     last-key [char] a = if -1e +time draw then
     last-key [char] s = if 1e +time draw then
     last-key [char] z = if -7e +time draw then
     last-key [char] x = if 7e +time draw then
     last-key [char] c = if -1e 48e f/ seek draw then
     last-key [char] v = if .1e 48e f/ seek draw then
   then
;

: main
   2457987.208333e in-time
   \ 2457986.5e in-time
   \ 2457832.6e in-time ( equinox )
   \ 2457924.5e in-time ( solstic )
   1024 1024 window
   begin handle-events again
;
main
