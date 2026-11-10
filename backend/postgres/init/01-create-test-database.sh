#!/bin/sh
set -eu

createdb --username "$POSTGRES_USER" --maintenance-db "$POSTGRES_DB" \
  --owner "$POSTGRES_USER" "$POSTGRES_TEST_DB"
