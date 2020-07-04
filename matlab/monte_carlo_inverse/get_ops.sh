#!/bin/bash
mua1line=`grep -w $1 $2 | head -n 2 | tail -n 1`
pattern='[0-9]+\.[0-9]+'
[[ $mua1line =~ $pattern ]]
echo ${BASH_REMATCH[0]}  
