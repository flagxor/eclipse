get-current
vocabulary julian also julian definitions

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

\ TODO!
: fdiv f/ floor ;

set-current

: julian. ( f -- )
  jJJ f!
  jJJ f@ jj f+ 4e jJJ f@ f* jBB f+ 146097e fdiv 3e f* 4e fdiv f+ jC f+ jf f!
  jr jf f@ f* jv f+ je f!
  je f@ jp fmod jr fdiv jg f!
  ju jg f@ f* jw f+ jh f!
  jh f@ js fmod ju fdiv 1e f+ jD f!
  jh f@ js fdiv jm f+ jn fmod 1e f+ jMM f!
  je f@ jp fdiv jy f- jn jm f+ jMM f@ f- jn fdiv f+ jYY f!
  ." [["
  jMM f@ f>s .
  ." / "
  jD f@ f>s .
  ." / "
  jYY f@ f>s .
  ." ]] "
;

previous
