#! /bin/bash
# Post-commit script to invoke removezombielocks in per-revision mode.
$1/hooks/removezombielocks $1 $2

