s" data/jpleph.blk" open-blocks

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
 819 10 4 2 planet earth-nuations
 899 10 4 3 planet moon-librations
\ 1019  0 0 3 planet moon-angv
\ 1019  0 0 1 planet tt-tdb

( vector ops )
fvariable vtx fvariable vty fvariable vtz
fvariable tsin fvariable tcos
: >vt vtz f! vty f! vtx f! ;
: vt> vtx f@ vty f@ vtz f@ ;
: v- >vt vtz f@ f- frot vtx f@ f- frot vty f@ f- frot ;
: fsquare fdup f* ;
: vdist2 fsquare fswap fsquare f+ fswap fsquare f+ ;
: vscale vtx f@ fover f* vtx f!
         vty f@ fover f* vty f!
         vtz f@ fswap f* vtz f! ;
: vunit >vt vt> vdist2 fsqrt 1e fswap f/ vscale vt> ;
: vdot >vt vtz f@ f* fswap vty f@ f* f+ fswap vtx f@ f* f+ ;
: >deg 180e f* pi f/ ;
: zrot ( v f -- ) fdup fcos tcos f! fsin tsin f! >vt
                  vtx f@ tcos f@ f* vty f@ tsin f@ f* f-
                  vtx f@ tsin f@ f* vty f@ tcos f@ f* f+ vtz f@ ;
: dayrot eph-time f@ .5e f+ 1e fmod pi f* 2e f* zrot ;
: longlat >vt vtx f@ vty f@ fatan2 pi f/ 2e f/ .5e f+
              vty f@ fsquare vtx f@ fsquare f+ fsqrt
              vtz f@ fatan2 pi f/ 2e f/ 2e f* ;

( Julian Day )
4716e fconstant jy   3e fconstant jv
1401e fconstant jj   5e fconstant ju
2e fconstant jm      153e fconstant js
12e fconstant jn     2e fconstant jw
4e fconstant jr       274277e fconstant jBB
1461e fconstant jp    -38e fconstant jC

fvariable jJJ
fvariable jf
fvariable je  fvariable jg  fvariable jh
fvariable jD  fvariable jMM  fvariable jYY
: fdiv f/ floor ;
: julian>dt ( f -- n )
  jJJ f!
  jJJ f@ jj f+ 4e jJJ f@ f* jBB f+ 146097e fdiv 3e f* 4e fdiv f+ jC f+ jf f!
  jr jf f@ f* jv f+ je f!
  je f@ jp fmod jr fdiv jg f!
  ju jg f@ f* jw f+ jh f!
  jh f@ js fmod ju fdiv 1e f+ jD f!
  jh f@ js fdiv jm f+ jn fmod 1e f+ jMM f!
  je f@ jp fdiv jy f- jn jm f+ jMM f@ f- jn fdiv f+ jYY f!
  jYY f@ 10000e f* jMM f@ 100e f* f+ jD f@ f+ f>s
;
