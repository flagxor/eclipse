#! /bin/bash

set -e

if ! dpkg -s imagemagick > /dev/null; then
  sudo apt-get install imagemagick
fi

mkdir -p data
curl https://upload.wikimedia.org/wikipedia/commons/thumb/8/83/Equirectangular_projection_SW.jpg/1024px-Equirectangular_projection_SW.jpg -o data/map.jpg
convert data/map.jpg -depth 8 -format BGRA -sample 1024x1024\! data/map.bgra

cd data
for x in {1550..2550..100}; do
  curl ftp://ssd.jpl.nasa.gov/pub/eph/planets/ascii/de436/ascp0${x}.436 -O
done
curl ftp://ssd.jpl.nasa.gov/pub/eph/planets/ascii/de436/header.436 -O
curl ftp://ssd.jpl.nasa.gov/pub/eph/planets/ascii/de436/testpo.436 -O
cd ..

./convert.fs
