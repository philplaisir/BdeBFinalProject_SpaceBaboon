// PHIL'S ARCHIVE AND NOTES 

/*
 
 Enemy movement
m_rB.AddForce(-m_rB.velocity.normalized * m_characterData.defaultAcceleration, ForceMode2D.Force);
 

 protected virtual void SlightPushFromObstructingObject(Collision2D collision)
 {
     //Vector3 direction = collision.transform.position - transform.position;
     //m_rB.AddForce(-direction * m_enemyUniqueData.obstructionPushForce, ForceMode2D.Force);
 }
 

protected virtual void MoveTowardsPlayer()
        {
            Vector3 playerPosition = m_playerObject.transform.position;

            m_movementDirection = (playerPosition - transform.position).normalized;
          //m_rB.AddForce(m_movementDirection * m_enemyUniqueData.defaultAcceleration  + or * bonus  //, ForceMode2D.Force);

if (m_movementDirection.magnitude > 0)
    RegulateVelocity();
        }
 





*/






/*
 
 //List<Vector3Int> positionsNearRadius = new List<Vector3Int>();
            //
            //foreach (var tilePos in m_tilemapRef.cellBounds.allPositionsWithin)
            //{
            //    if (m_tilemapRef.HasTile(tilePos))
            //    {
            //        float distance = Vector3Int.Distance(currentPlayerTilePos, tilePos);
            //        if (distance > radius - radiusThreshold && distance < radius + radiusThreshold)
            //        {
            //            positionsNearRadius.Add(tilePos);
            //        }
            //    }
            //}
            //
            //if (positionsNearRadius.Count > 0)
            //{
            //    int randomIndex = Random.Range(0, positionsNearRadius.Count);
            //    validTilePos = m_tilemapRef.CellToWorld(positionsNearRadius[randomIndex]) + new Vector3(0.5f, 0.5f, 0f);
            //}
            //else
            //{
            //    Debug.Log("No valid pos found on circle");
            //}

            //return validTilePos;
 
 
 
 
 
 
 */






/*
 
 To check for a group of tiles around a specific tile

private bool IsTilePositionValid(Vector3Int position)
        {
            TileBase mainTile = m_tilemapRef.GetTile(position);

            BoundsInt bounds = new BoundsInt(position.x - 4, position.y - 4, 0, 11, 11, 1);
            foreach (var positionToCheck in bounds.allPositionsWithin)
            {
                //Debug.Log(positionToCheck);
                if (m_obstacleTilemapRef.HasTile(positionToCheck))
                {
                    return false;
                }
            }

            return true;
        }
 
 
 
 
 */
