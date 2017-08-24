#! /usr/bin/env gforth

s" grf.fs" included

: main
   1024 768 window
   begin wait last-key 13 = if exit then again
;
main
bye
