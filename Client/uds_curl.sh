#!/bin/sh
while true; do
  start_timestamp=$(date +%s%3N)
  TID_END=1234567890123456789
  TID_START=8345678901234567890
  TRACE_ID=$(( (RANDOM % $(( TID_END - TID_START + 1))) + TID_START ))
  SID_END=1234567890123456789
  SID_START=4345678901234567890
  SPAN_ID_P=$(( (RANDOM % $(( SID_END - SID_START + 1))) + SID_START ))
  SPAN_ID_C=$(( SPAN_ID_P + 1 ))
  curl --unix-socket /var/run/datadog/apm.socket -X PUT -H "Content-type: application/json" \
    -d @- \
    http://localhost/v0.3/traces <<EOF_PAYLOAD
  [[
    {
      "trace_id": $TRACE_ID,
      "span_id": $SPAN_ID_P,
      "meta": {"fakefield": "me", "fakedata": "20", "env": "dev"},
      "name": "Span1",
      "resource": "/index",
      "service": "springtest4",
      "type": "web",
      "start": ${start_timestamp},
      "duration": 1200000
    }],
    [{
      "trace_id": $TRACE_ID,
      "span_id": $SPAN_ID_C,
      "parent_id": $SPAN_ID_P,
      "meta": {"fakefield": "me", "fakedata": "20", "env": "dev"},
      "name": "Span2",
      "resource": "/index2",
      "service": "springtest5",
      "type": "web",
      "start": ${start_timestamp},
      "duration": 1200000
    }]
  ]
EOF_PAYLOAD
  sleep 5
done
EOF
  #curl -X PUT -H "Content-type: application/json" \
  #        -d @- \
  #        http://localhost:8126/v0.3/traces <<EOF_PAYLOAD
