#! /usr/bin/env gforth

require eph.fs

100 constant max-line
create line max-line allot
variable fh
variable items
variable chunk-offset

: chunk, ( f -- ) chunk-offset @ eph@ f! 1 chunk-offset +! update ;

: d>e ( n -- )
  0 ?do line i + c@
    dup [char] D = if drop [char] E then
    line i + c!
  loop
;

: load-line
   line max-line fh @ read-line throw
   0= if drop 0 0 then
   dup d>e
   line swap evaluate
;

: process-chunk
  begin items @ 0<> while
    load-line
    fdepth dup 0 ?do dup i - 1- fpick chunk, loop
    dup 0 ?do fdrop loop
    items @ swap - items !
  repeat
;

: process
   begin
     load-line
     dup 0= if drop cr exit then
     nip items !
     process-chunk 
     42 emit
   again
;

: process-file ( str -- )
  2dup type cr
  r/o open-file throw fh !
  process
  fh @ close-file throw 
;

s" data/ascp01850.436" process-file
s" data/ascp01950.436" process-file
s" data/ascp02050.436" process-file
flush

bye
