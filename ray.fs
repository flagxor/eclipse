require math.fs
require vector.fs

get-current
vocabulary ray-words also ray-words definitions

vector source  vector direction   vector center fvariable radius
vector closest  fvariable distance   vector tsource

set-current

: ray ( target source ) source v! source v@ v- vunit direction v!  ;
: sphere ( center r ) radius f! center v! ;
: intersect
   source v@ center v@ v- tsource v!
   direction v@ vdup vdot
   direction v@ tsource v@ vdot 2e f*
   tsource v@ vdup vdot
   radius f@ fsquare f-
   quadratic fdrop distance f!
   dup 0>= if direction v@ distance f@ vscale source v@ v+ closest v! then
;

: hit ( -- v ) closest v@ ;

previous

