require math.fs

get-current
vocabulary vector-words also vector-words definitions

fvariable vtx fvariable vty fvariable vtz
fvariable tsin fvariable tcos
: >vt vtz f! vty f! vtx f! ;
: vt> vtx f@ vty f@ vtz f@ ;
fvariable scale
fvariable time

set-current

: vector create 3 floats allot does> ;
: v. >vt ." (" vtx f@ f. vty f@ f. vtz f@ f. ." ) " ;
: v+ >vt vtz f@ f+ frot vtx f@ f+ frot vty f@ f+ frot ;
: v- >vt vtz f@ f- frot vtx f@ f- frot vty f@ f- frot ;
: v@ dup f@ float+ dup f@ float+ f@ ;
: v! frot dup f! float+ fswap dup f! float+ f! ;
: vscale scale f! scale f@ f* frot scale f@ f* frot scale f@ f* frot ;
: vdist2 fsquare fswap fsquare f+ fswap fsquare f+ ;
: vdist vdist2 fsqrt ;
: vdup >vt vt> vt> ;
: vunit vdup vdist 1/f vscale ;
: vdot >vt vtz f@ f* fswap vty f@ f* f+ fswap vtx f@ f* f+ ;

: zrot ( v f -- ) fdup fcos tcos f! fsin tsin f! >vt
                  vtx f@ tcos f@ f* vty f@ tsin f@ f* f-
                  vtx f@ tsin f@ f* vty f@ tcos f@ f* f+ vtz f@ ;

\ drop this!
: longlat ( v tm -- x y )
   time f! >vt
   vty f@ vtx f@ fatan2 pi f/ 2e f/ .5e f- time f@ f+ 1e fmod
   vtz f@ vtx f@ vty f@ 0e vdist2 fsqrt fatan2  pi f/ fnegate .5e f+ ;

previous
