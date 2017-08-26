require math.fs
require vector.fs
require julian.fs

s" data/jpleph.blk" open-blocks

6371e fconstant earth-radius

variable eph-planet
: eph-offset eph-planet @ 3 cells + @ ;
: eph-coefficients eph-planet @ 2 cells + @ ;
: eph-segments eph-planet @ 1 cells + @ ;
: eph-components eph-planet @ 0 cells + @ ;

1018 constant chunk-size

: eph@ ( n -- a ) 128 /mod block swap floats + ;
: eph-chunks ( -- n ) get-block-fid file-size throw drop chunk-size floats / ;

variable eph-pos
: eph-chunk-offset ( -- n ) eph-pos @ chunk-size * ;
: eph-chunk-min ( -- f ) eph-chunk-offset eph@ f@ ;
: eph-chunk-max ( -- f ) eph-chunk-offset 1+ eph@ f@ ;

fvariable eph-time
: find-chunk ( a b f -- n )
   begin
     2dup + 2/ eph-pos !
     eph-time f@ eph-chunk-min f< if
       drop eph-pos @
     else
       eph-time f@ eph-chunk-max f> if
         nip eph-pos @ 1+ swap
       else
         2drop exit
       then
     then
   again
;

: in-time ( f -- ) eph-time f! ;
: +time ( f -- ) eph-time f@ f+ eph-time f! ;

create cheb-components 18 floats allot

fvariable cheb-t
fvariable cheb-tt
variable cheb-segment
: cheb-init-t
   eph-time f@ eph-chunk-min f- eph-chunk-max eph-chunk-min f- f/ cheb-t f!
   cheb-t f@ eph-segments s>f f* f>s cheb-segment !
   cheb-t f@ eph-segments s>f f* cheb-segment @ s>f f- 2e f* 1e f- cheb-tt f!
   \ eph-pos @ . cheb-segment @ . cheb-tt f@ f. cr
;
: cheb-components-init
   1e cheb-tt f@
   eph-coefficients 0 do
     fover i floats cheb-components + f!
     fdup cheb-tt f@ f* 2e f* frot f-
   loop
   fdrop fdrop
;
: eph-interp ( n -- f )
  0e
  eph-coefficients 0 do
    i floats cheb-components + f@
    dup eph-coefficients *
    cheb-segment @ eph-components * eph-coefficients * +
    i +
    eph-offset +
    eph-pos @ chunk-size * +
    eph@ f@ f* f+
  loop
  drop
;

: eph ( -- f* )
   0 eph-chunks find-chunk
   cheb-init-t
   cheb-components-init
   eph-components 0 do i eph-interp loop
;

: planet create ( start coefs secs comps -- ) , , , 1- ,
  does> eph-planet ! eph ;
   3 14 4 3 planet mercury
 171 10 2 3 planet venus
 231 13 2 3 planet earth
 309 11 1 3 planet mars
 342  8 1 3 planet jupiter
 366  7 1 3 planet saturn
 387  6 1 3 planet uranus
 405  6 1 3 planet neptune
 423  6 1 3 planet pluto
 441 13 8 3 planet moon
 753 11 2 3 planet sun
 819 10 4 2 planet earth-nutations
 899 10 4 3 planet moon-librations
\ 1019  0 0 3 planet moon-angv
\ 1019  0 0 1 planet tt-tdb
