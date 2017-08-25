get-current
vocabulary math also math definitions

fvariable a   fvariable b   fvariable c
fvariable d

set-current

: >deg 180e f* pi f/ ;

: fsquare ( n -- n^2 ) fdup f* ;

: quadratic ( a b c -- x1 x2 n )
   c f! b f! a f!
   b f@ fsquare 4e a f@ f* c f@ f* f-
   fdup 0e f> if
     fsqrt b f@ fnegate fover f- 2e f/ a f@ f/
     fswap b f@ fnegate fswap f+ 2e f/ a f@ f/ 2
     exit
   then
   fdup 0e f= if
     fdrop b f@ fnegate 2e f/ a f@ f/ 1
     exit
   then
   fdrop 0e 0e 0
;

previous
